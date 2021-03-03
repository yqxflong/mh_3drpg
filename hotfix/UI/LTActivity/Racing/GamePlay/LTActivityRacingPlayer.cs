//LTActivityRacingPlayer
//赛跑中的选手
//Johny

using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class ActivityRacingPlayer
    {
        public RacingPlayerData pd;

        #region  private
        private UnityEngine.Transform _transform;
        private MoveController _moveController;
        private Vector3 _maxMoveDis, _beginPos, _endPos;
        //当前buff
        private BaseActivityRacingBuff _curBuff = null;
        //buff序列
        private Queue<BaseActivityRacingBuff> _buffQueue = new Queue<BaseActivityRacingBuff>();
        private int _curSpeed = 1;
        private string _lastBroadCast = string.Empty;
        #endregion

        public bool Finished{get; private set;}

        public ActivityRacingPlayer(RacingPlayerData pd_)
        {
            pd = pd_;
        } 

        public void Destroy()
        {
            _curBuff?.Destroy();_curBuff = null;
            GameObject.Destroy(_transform.gameObject);_transform = null;
        }

        public bool Update(float dt)
        {
            if(Finished)
            {
                return true;
            }

            if(_curBuff == null)
            {
                Finished = NextBuff();
            }
            else if(_curBuff != null)
            {
                if(_curBuff.IsVictory())
                {
                    _curBuff.Update(dt);
                }
                else if(_curBuff.Update(dt))
                {
                    Finished = NextBuff();
                }
            }

            return Finished;
        }

        public bool IsVictory()
        {
            return _curBuff.IsVictory();
        }

        //广播消息
        public void BroadCastInRacing(string text)
        {
            if(_lastBroadCast.Equals(text))
            {
                return;
            }
            _lastBroadCast = text;
            LTActivityRacingManager.Instance.BroadCastInRacing(text);
        }

        //改变速度
        public void ChangeSpeed(int speed)
        {
            _curSpeed = speed;
        }

        //加载模型
        public void LoadModel(Transform root, Vector3 beginPos, Vector3 endPos)
        {
            _beginPos = beginPos;
            _endPos = endPos;
            _maxMoveDis = endPos - beginPos;
            string prefab_path = $"Bundles/Player/Variants/{pd.ModelName}";
            PoolModel.GetModelAsync(prefab_path, UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity, delegate (UnityEngine.Object obj, object param)
            {
                var variantObj = obj as UnityEngine.GameObject;
                var variantTrans = variantObj.transform;
                if (variantObj == null)
                {
                    EB.Debug.LogError("[Racing]Failed to create model!!!");
                    return;
                }
                variantTrans.parent = root.transform;
                variantTrans.localPosition = new Vector3(beginPos.x, beginPos.y, -50);
                _transform = variantTrans;
                _moveController = InitModel(variantObj);
            }, null);
        }

        //解析一组赛跑队列
        public void PrepareBuffData(ArrayList al, bool isWin)
        {
            for(int i = 0; i < al.Count; i++)
            {
                var unitData = al[i] as Hashtable;
                int order = System.Convert.ToInt16(unitData["index"]);
                int type =  System.Convert.ToInt16(unitData["id"]);
                float speed = float.Parse(unitData["speed"].ToString());
                _buffQueue.Enqueue(BaseActivityRacingBuff.CreateBuff(this, order, type, speed));
            }
            if(isWin)
            {
                _buffQueue.Enqueue(BaseActivityRacingBuff.CreateBuff(this, al.Count, (int)BaseActivityRacingBuff.BuffTYPE.tLocalVictory, 0));
            }
            else
            {
                _buffQueue.Enqueue(BaseActivityRacingBuff.CreateBuff(this, al.Count, (int)BaseActivityRacingBuff.BuffTYPE.tLocalLose, 0));
            }
        }

        //是否已经到达终点
        public bool HasReachedEnd(int sec)
        {
            return _buffQueue.Count <= sec;
        }

        //跳转到指定秒
        public bool JumpToSince(int since)
        {
            int counter = 0;
            while(_buffQueue.Count > 0)
            {
                var buff = _buffQueue.Dequeue();
                Vector3 nextMove = buff.MovePercent * _maxMoveDis;
                var fromPos = _transform.localPosition;
                var toPos = fromPos;
                toPos.x += nextMove.x;
                toPos = toPos.x <= _endPos.x ? toPos : _endPos;
                buff.SetHolder(_transform.gameObject, _moveController);
                buff.SkipKeepDis(fromPos, toPos);

                if(++counter == since)
                {
                    break;
                }
            }

            return _buffQueue.Count == 0;
        }

        //获取当前位置
        public Vector3 GetCurPosition()
        {
            return _transform.localPosition;
        }

        //下一个buff
        private bool NextBuff()
        {
            if(_buffQueue.Count > 0)
            {
                var buff = _buffQueue.Dequeue();
                _curBuff = buff;
                Vector3 nextMove = buff.MovePercent * _maxMoveDis;
                var fromPos = _transform.localPosition;
                var toPos = fromPos;
                toPos.x += nextMove.x;
                toPos = toPos.x <= _endPos.x ? toPos : _endPos;
                buff.SetHolder(_transform.gameObject, _moveController);
                buff.ChangeSpeed(_curSpeed);
                buff.Start(fromPos, toPos);
                return false;
            }

            return true;
        }
    
        #region About 模型
        private MoveController InitModel(GameObject variantObj)
        {
            CharacterVariant variant = variantObj.GetComponent<CharacterVariant>();
            variant.InstantiateCharacter();
            UnityEngine.GameObject character = variant.CharacterInstance;
            character.transform.SetParent(variant.transform);
            character.transform.localScale = UnityEngine.Vector3.one * 0.2f;
            character.transform.localRotation = UnityEngine.Quaternion.Euler(0, 90, -40);
            character.transform.localPosition = Vector3.zero;
            SetObjLayer(variantObj, GameEngine.Instance.ui3dLayer);
            MoveController mc = character.GetComponent<MoveController>();
            return mc;
        }

        private void SetObjLayer(UnityEngine.GameObject obj, int layer)
        {
            obj.transform.SetChildLayer(layer);
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; ++i)
            {
                renderers[i].gameObject.layer = layer;
                Renderer render = renderers[i];
                Material[] materials = render.materials;
                for (int j = 0; j < materials.Length; j++)
                {
                    Material ImageMaterial = new Material(materials[j]);
                    ImageMaterial.SetColor("_RimColor", new Color(0, 0, 0, 1f));
                    materials[j] = ImageMaterial;
                }
                render.materials = materials;
            }
        }
        #endregion
    }
}