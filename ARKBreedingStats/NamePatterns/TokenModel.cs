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
        public StatTokens te { get; set; } = new StatTokens();
        public StatTokens hp { get; set; } = new StatTokens();
        public StatTokens st { get; set; } = new StatTokens();
        public StatTokens to { get; set; } = new StatTokens();
        public StatTokens ox { get; set; } = new StatTokens();
        public StatTokens fo { get; set; } = new StatTokens();
        public StatTokens wa { get; set; } = new StatTokens();
        public StatTokens we { get; set; } = new StatTokens();
        public StatTokens dm { get; set; } = new StatTokens();
        public StatTokens sp { get; set; } = new StatTokens();
        public StatTokens fr { get; set; } = new StatTokens();
        public StatTokens cr { get; set; } = new StatTokens();
        public string[] highest_l { get; set; } = new string[Stats.StatsCount];
        public string[] highest_s { get; set; } = new string[Stats.StatsCount];
        public string[] highest_l_m { get; set; } = new string[Stats.StatsCount];
        public string[] highest_s_m { get; set; } = new string[Stats.StatsCount];
    }

    public class StatTokens
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
}