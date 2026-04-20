using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Plazma.Models;
using Plazma.Models.ClassAPI;
using Plazma.Models.NC;
using Plazma.Controllers;

namespace Plazma.Models.Services
{
    public class CncUploadService
    {
        private readonly admin _admin;

        // Разрешённые расширения CNC-файлов
        private static readonly HashSet<string> AllowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".cnc", ".nc"
        };

        // Максимальный размер загружаемого файла (20 МБ)
        private const int MaxFileSizeBytes = 20 * 1024 * 1024;

        public CncUploadService(admin admin)
        {
            _admin = admin;
        }

        /// <summary>
        /// Проверяет, допустимо ли расширение загружаемого файла.
        /// </summary>
        private static bool IsAllowedExtension(string fileName)
        {
            string ext = Path.GetExtension(fileName);
            return !string.IsNullOrEmpty(ext) && AllowedExtensions.Contains(ext);
        }

        /// <summary>
        /// Сохраняет загруженные файлы в целевой каталог после проверки имени, расширения и размера.
        /// </summary>
        public List<CncUploadName> UploadAndSave(IEnumerable<HttpPostedFileBase> uploads, string path)
        {
            List<CncUploadName> files = new List<CncUploadName>();
            if (uploads == null)
            {
                return files;
            }

            foreach (HttpPostedFileBase file in uploads)
            {
                if (file == null)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(file.FileName))
                {
                    throw new ArgumentException("Некорректное имя файла");
                }

                string fileName = Path.GetFileName(file.FileName);
                if (string.IsNullOrWhiteSpace(fileName) || fileName.Contains("..") || Path.IsPathRooted(fileName))
                {
                    throw new ArgumentException("Некорректное имя файла");
                }

                // Проверка разрешённого расширения файла
                if (!IsAllowedExtension(fileName))
                {
                    throw new ArgumentException("Недопустимый тип файла. Разрешены только .cnc и .nc");
                }

                // Проверка размера файла
                if (file.ContentLength > MaxFileSizeBytes)
                {
                    throw new ArgumentException("Файл превышает допустимый размер (20 МБ)");
                }

                string tmpFile = _admin.getNewFileName(fileName, path);
                if (string.IsNullOrWhiteSpace(tmpFile) || tmpFile.Contains("..") || Path.IsPathRooted(tmpFile))
                {
                    throw new ArgumentException("Некорректное имя файла");
                }

                string savedPath = Path.Combine(path, tmpFile);
                file.SaveAs(savedPath);
                files.Add(new CncUploadName(tmpFile, fileName));
            }

            return files;
        }

        public void SaveToDatabase(PartsClass parts, IEnumerable<CncUploadName> files, string path)
        {
            foreach (CncUploadName item in files)
            {
                string savedFilePath = Path.Combine(path, item.NewFileName);
                cnc parsed = new cnc(savedFilePath);
                PartsClass._CNC cncRecord = new PartsClass._CNC(
                    Filename: savedFilePath,
                    OriginalFile: item.OldFileName,
                    quantity: parsed.QuantityCut,
                    Tickness: cnc.gettiknessfromname(item.OldFileName),
                    Realtickness: cnc.gettiknessfromname(item.OldFileName),
                    material: PartsClass.GetMaterialFromName(item.OldFileName),
                    width: (int)(parsed.Sheet.maxX - parsed.Sheet.minX),
                    heigth: (int)(parsed.Sheet.maxY - parsed.Sheet.minY));

                int id = parts.AddCNCtoBD(cncRecord);
                foreach (cnc._part p in parsed.Parts)
                {
                    PartsClass._Part newPart = new PartsClass._Part(
                        name: p.Name,
                        quantity: p.quantity,
                        quantitysum: p.quantity * parsed.QuantityCut,
                        Quantitycutted: 0,
                        Tickness: cnc.gettiknessfromname(item.OldFileName),
                        SizeX: p.size.x.ToString(),
                        SizeY: p.size.y.ToString(),
                        CncId: id.ToString());
                    parts.AddPartToBD(newPart);
                }
            }
        }
    }
}
