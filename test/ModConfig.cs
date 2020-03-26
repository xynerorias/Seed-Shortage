namespace SeedShortageJA
{
    public class ModConfig
    {
        public bool PierreEnabled { get; set; } = true;
        public bool SandyEnabled { get; set; } = true;
        public bool MarnieEnabled { get; set; } = true;
        public bool HarveyEnabled { get; set; } = false;
        public bool ClintEnabled { get; set; } = false;
        public bool KrobusEnabled { get; set; } = true;
        public bool JojaEnabled { get; set; } = true;
        public bool TravellingEnabled { get; set; } = true;
        public string[] Exceptions { get; set; } =
        {
            "Parsnip Seeds",
            "Beefvine Seeds"
        };
    }
}
