using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectManager;

namespace ECMBase
{


    public struct DoubleRange
    {
        public double left;
        public double right;

        public DoubleRange(double left, double right)
        {
            this.left = left;
            this.right = right;
        }
    }


    public class ECMProject
    {
        public Dictionary<double, List<ECMLevel>> LevelList;
        public Dictionary<DoubleRange, List<ECMLevel>> LevelListRanged;
        public Dictionary<string, string> NameDic;

        public ECMOption option;

        public ECMProject()
        {
            this.LevelList = new();
            this.LevelListRanged = new();
            this.NameDic = new();
            this.option = new();
        }

        //생성자 고칠필요있음
    }

    public partial class ECMLevel
    {
        public string name;
        public double originlv;
        public Image image;

        public Color[] colors;

    }
}












