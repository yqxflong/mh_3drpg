using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class DialogueData
    {

        public int DialogueId;//对白id
        public DialogueStepData[] Steps;

        public DialogueData(int dialogueid)
        {
            DialogueId = dialogueid;
            Steps = new DialogueStepData[5];

        }

        public DialogueData(int dialogueid, int stepnum)
        {
            DialogueId = dialogueid;
            Steps = new DialogueStepData[stepnum];

        }
        public void AddStep(GM.DataCache.DialogueInfo steph)
        {
            DialogueStepData step = new DialogueStepData(steph);
            //Steps.Insert(3, step);
            if (step.StepId > Steps.Length || step.StepId < 1)
            {
                EB.Debug.LogError("Dialogue stepsnum is Illigal  id={0}" , DialogueId);
                return;
            }
            Steps[step.StepId - 1] = step;
        }


    }

    public class LobbyCameraData
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public float Size;
        public bool Orthographic;

        public Vector3 IconPosition;
        public Vector3 IconRotation;
    }

    public class DialogueStepData
    {
        public int DialogueId;//对白id
        public int StepId;//对白小步骤序号
        public string Icon;//说话者头像
        public string SpeakName;//说话人名字
        public int Layout;//布局
        public string Context;//对话内容
        public int Shade;//遮罩 0 1 2
        public int ShakeTime;//震屏
        public int StayTime;//停留时长
        public int StepNum;//对应的对白总步数
        public int FontSize;
        public LobbyCameraData LobbyCamera;//模型镜头数据

        public DialogueStepData()
        {
            DialogueId = 0;
            StepId = 0;
            Icon = "";
            SpeakName = "";
            Layout = 0;
            Context = "";
            Shade = 0;
            ShakeTime = 0;
            StayTime = 0;
            StepNum = 0;
            FontSize = 48;
            LobbyCamera = null;
        }

        public DialogueStepData(GM.DataCache.DialogueInfo steph)
        {
            DialogueId = steph.DialogueId;
            StepId = steph.StepId;
            Icon = steph.Icon;
            SpeakName = EB.Localizer.GetTableString(string.Format("ID_guide_dialogue_{0}_{1}_name", steph.DialogueId, steph.StepId), steph.Name);//;
            Layout = steph.Layout;
            Context = EB.Localizer.GetTableString(string.Format("ID_guide_dialogue_{0}_{1}_context", steph.DialogueId, steph.StepId), steph.Context);//;
            Shade = steph.Shade;
            ShakeTime = (int)steph.CameraShake;
            StayTime = 600;
            StepNum = steph.StepNum;
            FontSize = steph.FontSize;
            LobbyCamera = SetCameraData(steph.LobbyCamera);
        }

        private LobbyCameraData SetCameraData(string lobbyCamera)
        {
            if (lobbyCamera != null)
            {
                string[] sArray = lobbyCamera.Split(',');

                float[] fArray = LTUIUtil.ToFloat(sArray);
                Vector3 position = new Vector3(fArray[0], fArray[1], fArray[2]);
                Vector3 rotation = new Vector3(fArray[3], fArray[4], fArray[5]);
                float size = fArray[6];
                bool orthographic = ToBool(fArray[7]);

                Vector3 iconPosition = new Vector3(0, -6, 0);
                Vector3 iconRot = Vector3.zero;
                if (fArray.Length > 8)
                {
                    iconPosition = new Vector3(fArray[8], fArray[9], fArray[10]);
                    iconRot = new Vector3(fArray[11], fArray[12], fArray[13]);
                }
                LobbyCameraData lobbyCameraData = new LobbyCameraData
                {
                    Orthographic = orthographic,
                    Position = position,
                    Rotation = rotation,
                    Size = size,
                    IconPosition = iconPosition,
                    IconRotation = iconRot
                };

                return lobbyCameraData;
            }
            else
            {
                return new LobbyCameraData { Orthographic = true, Position = Vector3.zero, Rotation = Vector3.zero, Size = 0 };
            }
        }

        private bool ToBool(float a)
        {
            if (a != 0)
                return true;
            else
                return false;
        }
    }
}