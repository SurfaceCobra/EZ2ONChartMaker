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
                List<(double, ECMLevel)> imshiLevelList = new List<(double, ECMLevel)>();
                List<(DoubleRanged, ECMLevel)> imshiLevelRangedList = new List<(DoubleRanged, ECMLevel)>();

                foreach (var prelevel in preProject.script.LevelList)
                {
                    if(TryLoadLevel((prelevel.origin, prelevel.name), out ECMLevel level))
                    {
                        imshiLevelList.Add((prelevel.lv, level));
                    }
                }
                foreach (var prelevelranged in preProject.script.LevelRangedList)
                {
                    if (TryLoadLevel((prelevelranged.origin, prelevelranged.name), out ECMLevel level))
                    {
                        imshiLevelRangedList.Add((prelevelranged.lv, level));
                    }
                }

                bool TryLoadLevel((double origin, string name) prelevel, out ECMLevel level)
                {
                    level = new ECMLevel();
                    bool ok = true;

                    if (preProject.script.NameDic.TryGetValue(prelevel.name, out string fixedname))
                    {

                        level.name = fixedname;
                        if (preProject.Images.TryGetValue(fixedname, out Image image))
                        {
                            level.image = image;
                            level.originlv = prelevel.origin;
                        }
                        else
                        {
                            ok = false;
                            Log.Warning($"{prelevel.name} 이미지가 없음.");
                            //throw new ImageNotFoundException(fixedname);
                        }

                    }
                    else
                    {
                        ok = false;
                        Log.Warning($"{prelevel.name} 이름이 없음.");
                        //throw new NameNotFoundException(name);
                    }
                    return ok;
                }
                
                foreach (var (lv, level) in imshiLevelList)
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
                foreach (var (lvranged, level) in imshiLevelRangedList)
                {
                    if (project.LevelRangedList.ContainsKey(lvranged))
                    {
                        project.LevelRangedList[lvranged].Add(level);
                    }
                    else
                    {
                        project.LevelRangedList.Add(lvranged, new List<ECMLevel>(32) { level });
                    }
                }

            }


            return project;
        }
        public static ECMProject LoadProject(string projectPath) => LoadProject(PreECMDataLoader.LoadPreProject(projectPath));
        
        public static ECMProject LoadGlobalProject(PreECMProject preProject)
        {
            throw new NotImplementedException();
        }
    
    }

}
