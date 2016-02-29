using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    public partial class Pedigree : UserControl
    {
        public delegate void CreatureChangedEventHandler(Creature creature, bool forceUpdate);
        public List<Creature> creatures;
        public Creature creature;
        public List<Creature> children = new List<Creature>();
        private List<List<int[]>> lines = new List<List<int[]>>();
        private List<PedigreeCreature> pedigreeCreatures = new List<PedigreeCreature>();

        public Pedigree()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // paintBase
            base.OnPaint(e);

            e.Graphics.TranslateTransform(AutoScrollPosition.X, AutoScrollPosition.Y);
            if (creature != null)
                drawLines(e.Graphics);
        }

        public void drawLines(Graphics g)
        {
            System.Drawing.Pen myPen = new System.Drawing.Pen(System.Drawing.Color.Green, 3);
            myPen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            foreach (int[] line in lines[0])
            {
                if (line[4] == 1)
                    myPen.Color = Color.DarkRed;
                else if (line[4] == 2)
                    myPen.Color = Color.Green;
                else
                    myPen.Color = Color.LightGray;
                g.DrawLine(myPen, line[0], line[1], line[2], line[3]);
            }
            myPen.Color = Color.DarkGray;
            myPen.Width = 1;
            foreach (int[] line in lines[1])
            {
                g.DrawLine(myPen, line[0], line[1], line[2], line[3]);
            }
            if (children.Count > 0)
                g.DrawString("Descendants", new System.Drawing.Font("Arial", 14), new System.Drawing.SolidBrush(System.Drawing.Color.Black), 10, 170);
            myPen.Dispose();
        }


        /// <summary>
        /// call this function to create the pedigreeCreature-Elements
        /// </summary>
        public void createPedigree()
        {
            if (creature != null)
            {
                this.SuspendLayout();
                // clear old pedigreeCreatures        
                while (Controls.Count > 0)
                {
                    Controls[0].Dispose();
                }
                if (labelPedigreeInfo != null)
                {
                    labelPedigreeInfo.Dispose();
                    labelPedigreeInfo = null;
                }
                lines.Clear();
                lines.Add(new List<int[]>());
                lines.Add(new List<int[]>());

                // create ancestors
                createParentsChild(creature, 250, 60, true, true);
                if (creature.mother != null)
                {
                    if (createParentsChild(creature.mother, 10, 20, false))
                        lines[1].Add(new int[] { 231, 79, 250, 79 });
                }
                if (creature.father != null)
                {
                    if (createParentsChild(creature.father, 490, 20, false))
                        lines[1].Add(new int[] { 490, 79, 471, 159 });
                }

                // create descendants
                int y = 0;
                foreach (Creature c in children)
                {
                    PedigreeCreature pc = new PedigreeCreature(c);
                    pc.Location = new Point(10, 200 + 35 * y);
                    for (int s = 0; s < 7; s++)
                    {
                        if (creature.levelsWild[s] >= 0 && creature.levelsWild[s] == c.levelsWild[s])
                            lines[0].Add(new int[] { 10 + 38 + 28 * s, 200 + 35 * y + 6, 10 + 38 + 28 * s, 200 + 35 * y + 15, 0 });
                    }
                    Controls.Add(pc);
                    pc.CreatureChanged += new CreatureChangedEventHandler(setCreature);
                    y++;
                }
                this.ResumeLayout();
                this.Invalidate();
            }
        }

        private bool createParentsChild(Creature creature, int x, int y, bool drawWithNoParents = false, bool highlightCreature = false)
        {
            if (creature != null && (drawWithNoParents || creature.mother != null || creature.father != null))
            {
                // creature
                PedigreeCreature pc = new PedigreeCreature(creature);
                if (highlightCreature)
                    pc.highlight = true;
                pc.Location = new Point(x, y + 40);
                Controls.Add(pc);
                pc.CreatureChanged += new CreatureChangedEventHandler(setCreature);
                // mother
                if (creature.mother != null)
                {
                    pc = new PedigreeCreature(creature.mother);
                    pc.Location = new Point(x, y);
                    Controls.Add(pc);
                    pc.CreatureChanged += new CreatureChangedEventHandler(setCreature);
                }
                // father
                if (creature.father != null)
                {
                    pc = new PedigreeCreature(creature.father);
                    pc.Location = new Point(x, y + 80);
                    Controls.Add(pc);
                    pc.CreatureChanged += new CreatureChangedEventHandler(setCreature);
                }
                // gene-inheritance-lines
                // better: if father < mother: 1, if mother < father: -1
                int better;
                for (int s = 0; s < 7; s++)
                {
                    better = 0;
                    if (creature.mother != null && creature.father != null)
                    {
                        if (creature.mother.levelsWild[s] < creature.father.levelsWild[s])
                            better = -1;
                        else if (creature.mother.levelsWild[s] > creature.father.levelsWild[s])
                            better = 1;
                    }
                    if (creature.mother != null && creature.levelsWild[s] >= 0 && creature.levelsWild[s] == creature.mother.levelsWild[s])
                    {
                        lines[0].Add(new int[] { 38 + x + 28 * s, y + 33, 38 + x + 28 * s, y + 42, (better == -1 ? 1 : 2) });
                    }
                    if (creature.father != null && creature.levelsWild[s] >= 0 && creature.levelsWild[s] == creature.father.levelsWild[s])
                    {
                        lines[0].Add(new int[] { 38 + x + 28 * s, y + 86, 38 + x + 28 * s, y + 77, (better == 1 ? 1 : 2) });
                    }
                }
                return true;
            }
            return false;
        }

        public void setCreature(Creature centralCreature, bool forceUpdate = false)
        {
            if (centralCreature != null && (centralCreature != creature) || forceUpdate)
            {
                creature = centralCreature;
                // set children
                var children = from cr in creatures
                               where cr.motherGuid == creature.guid
                               || cr.fatherGuid == creature.guid
                               orderby cr.name ascending
                               select cr;
                this.children = children.ToList();
                createPedigree();
            }
        }
    }
}
