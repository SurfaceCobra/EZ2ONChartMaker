using ObjectManager;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECMBase
{
    public class ECMLoader
    {
        public static void SaveProjectOption(ECMOption option, string projectPath)
        {
            ObjectSaver.Save(option, Path.Combine(projectPath, "option.dat"));
        }


        public static ECMProject LoadProject(PreECMProject preProject)
        {
            ECMProject project = new ECMProject();

            project.option = preProject.option;

            project.NameDic = preProject.script.NameDic;

            {
                List<(double, ECMLevel)> pll = new();

                foreach (var (lv, originlv, name) in preProject.script.LEVELList)
                {
                    ECMLevel level = new ECMLevel();

                    if (preProject.script.NameDic.TryGetValue(name, out string fixedname))
                    {

                        level.name = fixedname;
                        if (preProject.Images.TryGetValue(fixedname, out Image image))
                        {
                            level.image = image;
                            level.originlv = originlv;
                        }
                        else
                        {
                            throw new ImageNotFoundException(fixedname);
                        }

                    }
                    else
                    {
                        throw new NameNotFoundException(name);
                    }

                    pll.Add((lv, level));
                }



                foreach (var (lv, level) in pll)
                {
                    if (project.LevelList.ContainsKey(lv))
                    {
                        project.LevelList[lv].Add(level);
                    }
                    else
                    {
                        project.LevelList.Add(lv, new List<ECMLevel>(32) { level });
                    }
                }
            }


            return project;
        }
        public static ECMProject LoadProject(string projectPath) => LoadProject(PreECMDataLoader.LoadPreProject(projectPath));
    }

}
