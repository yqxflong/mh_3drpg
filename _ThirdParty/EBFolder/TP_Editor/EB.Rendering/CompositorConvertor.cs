using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace EB.Rendering
{
    public class CompositorConvertor
    {
        public const  string ParticleUberName = "EBG/Particle/Uber";
        public const  string EnviroUberName = "EBG/Enviro/Uber/Uber";
        public const  string EnviroUberCutoutName = "EBG/Enviro/Uber/UberCutout";
        public const  string EnviroUberFadeName = "EBG/Enviro/Uber/UberFade";
        public const  string EnviroUberOwnLightMapName = "EBG/Enviro/Uber/UberOwnLightMap";
        public const  string EnviroUberT4MName = "EBG/Enviro/Uber/UberT4M";
        public const  string CharactorUberName = "EBG/Character/Uber";
        public const  string CharactorUberCutoutName = "EBG/Character/UberCutout";

        public const  string HiddenParticleUberName = "Hidden/Particle/Uber";
        public const  string HiddenEnviroUberName = "Hidden/Enviro/Uber/Uber";
        public const  string HiddenEnviroUberCutoutName = "Hidden/Enviro/Uber/UberCutout";
        public const  string  HiddenEnviroUberFadeName = "Hidden/Enviro/Uber/UberFade";
        public const  string HiddenEnviroUberOwnLightMapName = "Hidden/Enviro/Uber/UberOwnLightMap";
        public const  string HiddenEnviroUberT4MName = "Hidden/Enviro/Uber/UberT4M";
        public const  string HiddenCharactorUberName = "Hidden/Character/Uber";
        public const  string HiddenCharactorUberCutoutName = "Hidden/Character/UberCutout";

        public static void ConvertToAndBack(Material mat)
        {
            if (IsHiddenShader(mat))
            {
                ConvertToUber(mat);
            }
            else if (IsUberShader(mat))
            {
                ConvertUberShader(mat);
            }
        }

        public static bool IsHiddenShader(Material mat)
        {
            if (mat.shader.name.Contains(HiddenCharactorUberCutoutName))
            {
                return true;
            }
            else if (mat.shader.name.Contains(HiddenCharactorUberName))
            {
                return true;
            }
            else if (mat.shader.name.Contains(HiddenEnviroUberCutoutName))
            {
                return true;
            }
            else if (mat.shader.name.Contains(HiddenEnviroUberFadeName))
            {
                return true;
            }
            else if (mat.shader.name.Contains(HiddenEnviroUberOwnLightMapName))
            {
                return true;
            }
            else if (mat.shader.name.Contains(HiddenEnviroUberT4MName))
            {
                return true;
            }
            else if (mat.shader.name.Contains(HiddenEnviroUberName))
            {
                return true;
            }
            else if (mat.shader.name.Contains(HiddenParticleUberName))
            {
                return true;
            }
            return false;
        }

        public static bool IsUberShader(Material mat)
        {
            switch (mat.shader.name)
            {
                case CharactorUberCutoutName:
                case CharactorUberName:
                    return true;
                case EnviroUberOwnLightMapName:
                case EnviroUberT4MName:
                case EnviroUberCutoutName:
                case EnviroUberFadeName:
                case EnviroUberName:
                    return true;
                case ParticleUberName:
                    return true;
                default:
                    return false;
            }
        }

        public static void ConvertUberShader(Material mat)
        {
            switch (mat.shader.name)
            {
                case CharactorUberCutoutName:
                case CharactorUberName:
                    CharacterUberCompositor.Instance.Convert(mat);
                    break;
                case EnviroUberOwnLightMapName:
                case EnviroUberT4MName:
                case EnviroUberCutoutName:
                case EnviroUberFadeName:
                case EnviroUberName:
                    EnviroUberCompositor.Instance.Convert(mat);
                    break;
                case ParticleUberName:
                    ParticleUberCompositor.Instance.Convert(mat);
                    break;
                default:
                    break;
            }
        }

        public static void ConvertToUber(Material mat)
        {
            if (mat.shader.name.Contains(HiddenCharactorUberCutoutName))
            {
                mat.shader = Shader.Find(CharactorUberCutoutName);
            }
            else if (mat.shader.name.Contains(HiddenCharactorUberName))
            {
                mat.shader = Shader.Find(CharactorUberName);
                //mat.shader.maximumLOD = Shader.Find(CharactorUberName).maximumLOD;
            }
            else if (mat.shader.name.Contains(HiddenEnviroUberOwnLightMapName))
            {
                mat.shader = Shader.Find(EnviroUberOwnLightMapName);
            }
            else if (mat.shader.name.Contains(HiddenEnviroUberT4MName))
            {
                mat.shader = Shader.Find(EnviroUberT4MName);
            }
            else if (mat.shader.name.Contains(HiddenEnviroUberCutoutName))
            {
                mat.shader = Shader.Find(EnviroUberCutoutName);
            }
            else if (mat.shader.name.Contains(HiddenEnviroUberFadeName))
            {
                mat.shader = Shader.Find(EnviroUberFadeName);
            }
            else if (mat.shader.name.Contains(HiddenEnviroUberName))
            {
                mat.shader = Shader.Find(EnviroUberName);
                //mat.shader.maximumLOD = Shader.Find(EnviroUberName).maximumLOD;
            }
            else if (mat.shader.name.Contains(HiddenParticleUberName))
            {
                mat.shader = Shader.Find(ParticleUberName);
                //mat.shader.maximumLOD = Shader.Find(ParticleUberName).maximumLOD;
            }
        }
    }
}
