namespace Plazma.Models.NC
{
    public static class SpeedCatalog
    {
        public static speed GetRecommendedSpeed(string tickn)
        {
            if (tickn == "1") return new speed(Process._Process.PL30, 8, 5000, 12000, 240);
            if (tickn == "1,2") return new speed(Process._Process.PL50, 8, 4150, 12000, 240);
            if (tickn == "1_2") return new speed(Process._Process.PL50, 8, 4150, 12000, 240);
            if (tickn == "1,5") return new speed(Process._Process.PL50, 8, 3200, 12000, 240);
            if (tickn == "1_5") return new speed(Process._Process.PL50, 8, 3200, 12000, 240);
            if (tickn == "2") return new speed(Process._Process.PL50, 8, 9800, 12000, 240);
            if (tickn == "3") return new speed(Process._Process.PL80, 8, 6145, 12000, 240);
            if (tickn == "4") return new speed(Process._Process.PL80, 8, 3670, 12000, 240);
            if (tickn == "5") return new speed(Process._Process.PL80, 8, 4760, 12000, 240);
            if (tickn == "6") return new speed(Process._Process.PL130, 8, 4035, 12000, 240);
            if (tickn == "7") return new speed(Process._Process.PL130, 8, 3700, 12000, 240);
            if (tickn == "8") return new speed(Process._Process.PL200, 8, 3360, 12000, 240);
            if (tickn == "9") return new speed(Process._Process.PL200, 8, 4000, 12000, 240);
            if (tickn == "10") return new speed(Process._Process.PL200, 8, 3460, 12000, 240);
            if (tickn == "11") return new speed(Process._Process.PL200, 8, 3200, 12000, 240);
            if (tickn == "12") return new speed(Process._Process.PL200, 8, 3060, 12000, 240);
            if (tickn == "14") return new speed(Process._Process.PL200, 8, 2800, 12000, 240);
            if (tickn == "15") return new speed(Process._Process.PL200, 8, 2275, 12000, 240);
            if (tickn == "16") return new speed(Process._Process.PL200, 8, 2050, 12000, 240);
            if (tickn == "18") return new speed(Process._Process.PL200, 8, 1900, 12000, 240);
            if (tickn == "20") return new speed(Process._Process.PL200, 8, 1575, 12000, 240);
            if (tickn == "22") return new speed(Process._Process.PL200, 12, 1400, 12000, 240);
            if (tickn == "25") return new speed(Process._Process.Gas, 12, 550, 12000, 240);
            if (tickn == "28") return new speed(Process._Process.Gas, 12, 550, 12000, 240);
            if (tickn == "30") return new speed(Process._Process.Gas, 12, 500, 12000, 240);
            if (tickn == "32") return new speed(Process._Process.Gas, 15, 490, 12000, 240);
            if (tickn == "36") return new speed(Process._Process.Gas, 15, 460, 12000, 240);
            if (tickn == "40") return new speed(Process._Process.Gas, 15, 450, 12000, 240);
            if (tickn == "45") return new speed(Process._Process.Gas, 18, 450, 12000, 240);
            if (tickn == "50") return new speed(Process._Process.Gas, 25, 420, 12000, 240);
            if (tickn == "60") return new speed(Process._Process.Gas, 25, 320, 12000, 240);
            if (tickn == "70") return new speed(Process._Process.Gas, 30, 310, 12000, 240);
            if (tickn == "80") return new speed(Process._Process.Gas, 30, 280, 12000, 240);
            if (tickn == "90") return new speed(Process._Process.Gas, 40, 280, 12000, 240);
            if (tickn == "100") return new speed(Process._Process.Gas, 40, 240, 12000, 240);
            if (tickn == "110") return new speed(Process._Process.Gas, 50, 240, 12000, 240);
            if (tickn == "120") return new speed(Process._Process.Gas, 60, 240, 12000, 240);
            if (tickn == "130") return new speed(Process._Process.Gas, 70, 200, 12000, 240);
            if (tickn == "140") return new speed(Process._Process.Gas, 90, 200, 12000, 240);
            if (tickn == "150") return new speed(Process._Process.Gas, 100, 180, 12000, 240);
            if (tickn == "160") return new speed(Process._Process.Gas, 120, 180, 12000, 240);
            return new speed(Process._Process.PL200, 12, 3000, 12000, 240);
        }
    }
}
