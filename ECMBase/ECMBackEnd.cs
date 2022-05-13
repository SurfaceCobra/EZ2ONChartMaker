using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectManager;

namespace ECMBase
{

    public class ECMDataLoader
    {


        public static void SaveProjectOption(ECMOption option, string projectPath)
        {
            ObjectSaver.Save(option,Path.Combine(projectPath,"option.dat"));
        }
        public static ECMProject LoadProject(string projectPath)
        {
            List<string> filepaths = Directory.EnumerateFiles(Path.Combine(projectPath, "data")).ToList();

            List<string> scriptpaths = filepaths.FindAll((fullpath) => Path.GetExtension(fullpath) == ".txt");
            //List<string> datapaths = filepaths.FindAll((fullpath) => Path.GetExtension(fullpath) == ".dat");
            List<ECMScript> scripts = scriptpaths.Select<string, ECMScript>((path) => LoadScript(path)).ToList();

            List<string> imagefilepaths = Directory.EnumerateFiles(Path.Combine(projectPath, "image")).ToList();

            List<string> imagepaths = imagefilepaths.FindAll((fullpath) => Path.GetExtension(fullpath) == ".png");
            Dictionary<string,Image> images = new Dictionary<string, Image>( imagepaths.Select<string, KeyValuePair<string,Image> > ((path) => KeyValuePair.Create(Path.GetFileNameWithoutExtension(path).ToLower(),LoadImage(path))));

            

            ECMScript script2 = new ECMScript();


            foreach(var scr in scripts)
            {
                script2.LEVELList.AddRange(scr.LEVELList);
                foreach(var pair in scr.NAMEDic)
                {
                    script2.NAMEDic.Add(pair.Key,pair.Value);
                }
            }

            ECMScript script3 = new ECMScript();

            foreach (var scr2 in script2.LEVELList)
            {
                try
                {
                    script3.LEVELList.Add((scr2.level, scr2.origin, script2.NAMEDic[scr2.name]));
                }
                catch
                {
                    throw new Exception($"{scr2.name} 이름이 등록되지 않았습니다.");
                }
            }
            script3.NAMEDic = script2.NAMEDic;




            ECMOption option = (ObjectSaver.Load(Path.Combine(projectPath,"option.dat")) as ECMOption);




            return ECMProject.Create(script3,images,option);

            //이부분 코드 문제있음
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

        public static ECMScript LoadScript(string scriptPath)
        {
            ECMScript script = new ECMScript();

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


            while(true)
            {

                switch (ReadNext(), state, sms)
                {
                    case ("\0", _,_):
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

                        switch(op, ACount)
                        {
                            case ("LABEL",1):
                                state = State.LABEL;
                                sms = SmallState.LineEnd;
                                ACount = 0;
                                break;

                            case ("LEVEL", 2):
                                state = State.LEVEL;
                                sms = SmallState.EndedReadingA;
                                ACount--;
                                break;

                            case ("NAME", 2):
                                state = State.NAME;
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

                        if(ACount==0)
                        {
                            sms = SmallState.LineEnd;
                        }

                        break;

                    case (";", State s, SmallState.LineEnd or SmallState.Normal):


                        switch(s)
                        {
                            case State.LABEL:
                                break;

                            case State.NAME:
                                BStacks.ForEach((str)=>str=str.ToLower());
                                script.NAMEDic.Add(string.Concat(BStacks).ToLower(),  AStacks[0]);

                                break;

                            case State.LEVEL:
                                BStacks.ForEach((str) => str = str.ToLower());
                                script.LEVELList.Add((double.Parse(AStacks[0]), double.Parse(BStacks[0]), string.Concat(BStacks.Skip(1)).ToLower()));
                                break;
                        }



                        BStacks.Clear();
                        break;

                    case (string op,_, SmallState.LineEnd or SmallState.Normal):

                        BStacks.Add(op);
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

    public class ECMProject : ECMScript
    {
        public Dictionary<string, Image> Images;

        public static ECMProject Create()
        {
            ECMProject project = new ECMProject();
            project.option.coverImage.parentProject = project;
            return project;
        }

        public static ECMProject Create(ECMScript Script, Dictionary<string, Image> Images, ECMOption option)
        {
            ECMProject project = new ECMProject( Script,  Images,  option);
            project.option.coverImage.parentProject = project;
            return project;
        }


        ECMProject()
        {
            this.LEVELList = new();
            this.NAMEDic = new();
            this.Images = new();
            this.option = new();
        }
        ECMProject(ECMScript Script, Dictionary<string,Image> Images, ECMOption option)
        {
            this.LEVELList = Script.LEVELList;
            this.NAMEDic = Script.NAMEDic;
            this.Images = Images;
            this.option = option;
        }




        public ECMOption option;
    }


    public class ECMData
    {

    }

    public class ECMScript
    {
        public List<(double level, double origin, string name)> LEVELList = new();

        public Dictionary<string, string> NAMEDic = new();
    }
}


