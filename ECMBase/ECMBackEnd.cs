using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectManager;

namespace ECMBase
{
    public record struct DoubleRanged(double left, double right)
    {
        public static implicit operator (double left, double right)(DoubleRanged value)
        {
            return (value.left, value.right);
        }

        public static implicit operator DoubleRanged((double left, double right) value)
        {
            return new DoubleRanged(value.left, value.right);
        }
    }


    public class ECMProject
    {
        public Dictionary<double, List<ECMLevel>> LevelList;
        public Dictionary<DoubleRanged, List<ECMLevel>> LevelRangedList;
        public Dictionary<string, string> NameDic;

        public ECMOption option;

        public ECMProject()
        {
            this.LevelList = new();
            this.LevelRangedList = new();
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












