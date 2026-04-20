using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace GoogleSpreadSheetsWorking
{
    public class GSSW
    {
        string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        string ApplicationName = "Google Sheets API .NET Plasma_Server";
        UserCredential credential;
        public GSSW()
        {
            FileStream stream;
            try
            {
                stream =
                    new FileStream("credentials.json", FileMode.Open, FileAccess.Read);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
                 stream =new FileStream("credentials.json", FileMode.Open, FileAccess.Read);
            
            using (var stream2 =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define request parameters.
            //https://docs.google.com/spreadsheets/d/12N_OjdadX5T3oMX4jDSx5OQ1chCbF7S_DrWuyGs6i94/edit#gid=0
            String spreadsheetId = "1qxBM0zzPJkPfMN5om-E7qLChnwgPzOGYlFoTkzW36n0";
            String range = "";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);

            // Prints the names and majors of students in a sample spreadsheet:
            // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
            var t_12 = request.RequestParameters.Keys;
            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;
            object resp = response;
        }
        public void testWrite(SpreadsheetsResource.ValuesResource.GetRequest request)
        {
            var t_12 = request.RequestParameters.Keys;
            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;
            object resp = response;
        }
        public void testRead(SpreadsheetsResource.ValuesResource.GetRequest request)
        {
            var t_12 = request.RequestParameters.Keys;
            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;
            object resp = response;
        }

    }
}
