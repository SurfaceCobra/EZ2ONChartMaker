using ObjectManager;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECMBase
{

    public class PreECMDataLoader
    {



        public static PreECMProject LoadPreProject(string projectPath)
        {

            List<string> filepaths = Directory.EnumerateFiles(Path.Combine(projectPath, "data")).ToList();

            List<string> scriptpaths = filepaths.FindAll((fullpath) => Path.GetExtension(fullpath) == ".txt");
            //List<string> datapaths = filepaths.FindAll((fullpath) => Path.GetExtension(fullpath) == ".dat");
            List<PreECMScript> scripts = scriptpaths.Select<string, PreECMScript>((path) => LoadPreScript(path)).ToList();


            List<string> imagefilepaths = Directory.EnumerateFiles(Path.Combine(projectPath, "image")).ToList();

            List<string> imagepaths = imagefilepaths.FindAll((fullpath) => Path.GetExtension(fullpath) == ".png");




            var Images = new Dictionary<string, Image>(imagepaths.Select<string, KeyValuePair<string, Image>>((path) => KeyValuePair.Create(Path.GetFileNameWithoutExtension(path).ToLower(), LoadImage(path))));
            ECMOption option;
            try
            {
                option = (ObjectSaver.Load(Path.Combine(projectPath, "option.dat")) as ECMOption) ?? new ECMOption();
            }
            catch
            {
                option = new ECMOption();
            }
            
            var script = PreECMScript.Concat(scripts);
            return new PreECMProject(Images, option, script);

            //문제있음
        }

        enum State
        {
            LABEL,
            NAME,
            LEVEL,
            LEVELRANGE,
        }

        enum SmallState
        {
            Normal,
            ReadingA,
            EndedReadingA,
            LineEnd
        }

        public static PreECMScript LoadPreScript(string scriptPath)
        {
            PreECMScript script = new PreECMScript();

            ScriptReader reader = new ScriptReader(File.ReadAllText(scriptPath));

            State state = State.LABEL;
            SmallState sms = SmallState.LineEnd;
            int ACount = 0;

            List<string> AStacks = new List<string>(16);
            List<string> BStacks = new List<string>();


            Ranged<string> STRR;
            string ReadNext()
            {
                if (reader.TryReadNext(out STRR))
                {
                    return STRR.o;
                }
                else
                {
                    return "\0";
                }
            }


            while (true)
            {

                switch (ReadNext(), state, sms)
                {
                    case ("\0", _, _):
                        return script;
                        break;


                    case ("@", _, SmallState.LineEnd):
                        sms = SmallState.ReadingA;
                        AStacks.Clear();
                        goto AChecker;


                    case ("@", _, SmallState.ReadingA):
                    AChecker:
                        ACount++;
                        break;

                    case (string op, _, SmallState.ReadingA):

                        switch (op, ACount)
                        {
                            case ("LABEL", 1):
                                state = State.LABEL;
                                goto LineEnd;

                            case ("LEVEL", 2):
                                state = State.LEVEL;
                                goto EndedReadingA;

                            case ("NAME", 2):
                                state = State.NAME;
                                goto EndedReadingA;

                            case ("LEVELRANGE", 3):
                                state = State.LEVELRANGE;
                                goto EndedReadingA;

                            LineEnd:
                                sms = SmallState.LineEnd;
                                ACount=0;
                                break;

                            EndedReadingA:
                                sms = SmallState.EndedReadingA;
                                ACount--;
                                break;

                            default:
                                throw new Exception($"{STRR.range} 위치에서 오류 발생");
                                break;
                        }

                        break;

                    case (string op, _, SmallState.EndedReadingA):
                        if (ACount > 0)
                        {
                            AStacks.Add(op);
                            ACount--;
                        }
                        else
                        {
                            throw new Exception($"{STRR.range} 위치에서 오류 발생");
                        }

                        if (ACount == 0)
                        {
                            sms = SmallState.LineEnd;
                        }

                        break;

                    case (";", State s, SmallState.LineEnd or SmallState.Normal):


                        switch (s)
                        {
                            case State.LABEL:
                                break;

                            case State.NAME:
                                script.NameDic.Add
                                    (
                                    string.Concat(BStacks).ToLower(),
                                    AStacks[0]
                                    );
                                break;

                            case State.LEVEL:
                                script.LevelList.Add
                                    ((
                                    double.Parse(AStacks[0]),
                                    double.Parse(BStacks[0]),
                                    string.Concat(BStacks.Skip(1)).ToLower()
                                    ));
                                break;

                            case State.LEVELRANGE:
                                script.LevelRangedList.Add
                                    ((
                                    (double.Parse(AStacks[0]), double.Parse(AStacks[1])),
                                    double.Parse(BStacks[0]),
                                    string.Concat(BStacks.Skip(1)).ToLower()
                                    ));
                                break;

                        }



                        BStacks.Clear();
                        break;

                    case (string op, _, SmallState.LineEnd or SmallState.Normal):

                        BStacks.Add(op.ToLower());
                        break;

                    default:
                        throw new Exception($"{STRR.range} 위치에서 오류 발생");


                        break;
                }
            }

        }

        public static Image LoadImage(string scriptPath)
        {
            return Image.FromFile(scriptPath);
        }
    }

    public class PreECMProject
    {
        public Dictionary<string, Image> Images;

        public ECMOption option;

        public PreECMScript script;

        public PreECMProject()
        {
            this.Images = new();
            this.option = new();
            this.script = new();
        }
        public PreECMProject(Dictionary<string, Image> Images, ECMOption option, PreECMScript script)
        {
            this.Images = Images;
            this.option = option;
            this.script = script;
        }
    }

    public class PreECMScript
    {
        public List<PreECMLevel> LevelList = new();

        public List<PreECMLevelRanged> LevelRangedList = new();

        public Dictionary<string, string> NameDic = new();

        public static PreECMScript Concat(params PreECMScript[] scripts) => Concat(scripts.AsEnumerable());
        public static PreECMScript Concat(IEnumerable<PreECMScript> scripts)
        {
            PreECMScript origin = new PreECMScript();
            foreach (var script in scripts)
            {
                origin.LevelList.AddRange(script.LevelList);
                origin.LevelRangedList.AddRange(script.LevelRangedList);

                foreach (var pair in script.NameDic)
                {
                    origin.NameDic.Add(pair.Key, pair.Value);
                }
            }
            return origin;
        }
    }

    public record struct PreECMLevel(double lv, double origin, string name)
    {
        public static implicit operator (double level, double origin, string name)(PreECMLevel value)
        {
            return (value.lv, value.origin, value.name);
        }

        public static implicit operator PreECMLevel((double lv, double origin, string name) value)
        {
            return new PreECMLevel(value.lv, value.origin, value.name);
        }
    }

    public record struct PreECMLevelRanged(DoubleRanged lv, double origin, string name)
    {
        public static implicit operator (DoubleRanged lv, double origin, string name)(PreECMLevelRanged value)
        {
            return (value.lv, value.origin, value.name);
        }

        public static implicit operator PreECMLevelRanged((DoubleRanged lv, double origin, string name) value)
        {
            return new PreECMLevelRanged(value.lv, value.origin, value.name);
        }
    }
}
