using ARKBreedingStats.Library;

namespace ARKBreedingStats.NamePatterns
{
    public class TokenModel
    {
        internal int? effimp_value;

        public string species { get; set; }
        public string spcsnm { get; set; }
        public string firstwordofoldest { get; set; }
        public string owner { get; set; }
        public string tribe { get; set; }
        public string server { get; set; }
        public Sex sex { get; set; }
        public string sex_short { get; set; }
        public string effimp_short { get; set; }
        public int index { get; set; }
        public string oldname { get; set; }
        public string sex_lang { get; set; }
        public string sex_lang_short { get; set; }
        public string sex_lang_gen { get; set; }
        public string sex_lang_short_gen { get; set; }
        public float toppercent { get; set; }
        public int baselvl { get; set; }
        public int levelpretamed { get; set; }
        public string effimp { get; set; }
        public int muta { get; set; }
        public int mutam { get; set; }
        public int mutap { get; set; }
        public int gen { get; set; }
        public string gena { get; set; }
        public int nr_in_gen { get; set; }
        public int nr_in_gen_sex { get; set; }
        public int ln { get; set; }
        public int tn { get; set; }
        public int sn { get; set; }
        public string arkid { get; set; }
        public bool alreadyexists { get; set; }
        public bool isflyer { get; set; }
        public int genn { get; set; }
        public int rnd { get; set; }
        public string dom { get; set; }
        public CreatureStatus status { get; set; }
        public StatModel te { get; set; } = new StatModel();
        public StatModel hp { get; set; } = new StatModel();
        public StatModel st { get; set; } = new StatModel();
        public StatModel to { get; set; } = new StatModel();
        public StatModel ox { get; set; } = new StatModel();
        public StatModel fo { get; set; } = new StatModel();
        public StatModel wa { get; set; } = new StatModel();
        public StatModel we { get; set; } = new StatModel();
        public StatModel dm { get; set; } = new StatModel();
        public StatModel sp { get; set; } = new StatModel();
        public StatModel fr { get; set; } = new StatModel();
        public StatModel cr { get; set; } = new StatModel();
        /// <summary>
        /// Stat levels ordered descended by level height.
        /// </summary>
        public string[] highest_l { get; set; } = new string[Stats.StatsCount];
        /// <summary>
        /// Stat indices ordered descended by level height.
        /// </summary>
        public string[] highest_s { get; set; } = new string[Stats.StatsCount];
        /// <summary>
        /// Mutation stat levels ordered descended by level height.
        /// </summary>
        public string[] highest_l_m { get; set; } = new string[Stats.StatsCount];
        /// <summary>
        /// Mutation stat indices ordered descended by level height.
        /// </summary>
        public string[] highest_s_m { get; set; } = new string[Stats.StatsCount];
        public ColorModel[] colors { get; set; } = new ColorModel[6];
    }

    public class StatModel
    {
        public int level { get; set; }
        public double level_vb { get; set; }
        public bool istop { get; set; }
        public bool isnewtop { get; set; }
        public bool islowest { get; set; }
        public bool isnewlowest { get; set; }
        public bool istop_m { get; set; }
        public bool isnewtop_m { get; set; }
        public bool islowest_m { get; set; }
        public bool isnewlowest_m { get; set; }
        public int level_m { get; set; }
    }

    public class ColorModel
    {
        public byte id { get; set; }
        public string name { get; set; }
        public bool used { get; set; }
        public string @new { get; set; }
    }
}