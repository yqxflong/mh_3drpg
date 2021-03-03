//JohnySpline
//Author: Johny
//用于实现动态物体的平滑曲线运动
//===翻版必究===

using UnityEngine;
using System.Collections.Generic;

namespace Johny{
    public class FollowPath : MonoBehaviour
    {
        public enum FollowType
        {//定义一个移动类型的枚举
            MoveTowards,
            Lerp
        }
        public FollowType type = FollowType.MoveTowards;//默认为直接朝向移动
        public float time = 60;//获得点的次数
        public float Speed = 100;//速度
        public float maxDistanceTOGoal = 0.1f;//判断物体是否到达某个路径点
        public float zangle = 100f;//旋转角度
        public int direction = -1;//正往右，负往左
        public float LengthForX = 1f;
        Vector3 starttrans;
        public Transform target;
        CRSpline srs;
        int count;
        private void Start()
        {
            this.gameObject.SetActive(false);
            starttrans = this.transform.position;

        }
        
        private void OnEnable()
        {
            if (target == null) { return; }
            transform.position = starttrans;
            float middlex = starttrans.x > target.position.x ? target.position.x - LengthForX : starttrans.x - LengthForX;
            float middley = (starttrans.y + target.position.y) / 2;
            Vector3 middle = new Vector3(middlex, middley, 0);
            List<Vector3> pos = new List<Vector3> { starttrans, middle, target.position };
            srs = new CRSpline(pos.ToArray());
            count = 1;
        }

        void Update()
        {
            if (target == null) { return; }

            if (count <= time)
            {
                if (type == FollowType.MoveTowards)
                {//使用Vector3.MoveTowards移动
                    transform.position = Vector3.MoveTowards(transform.position, srs.Interp(count / time), Time.deltaTime * Speed);

                }
                else if (type == FollowType.Lerp)
                {
                    //使用Vector3.Lerp移动
                    transform.position = Vector3.Lerp(transform.position, srs.Interp(count / time), Time.deltaTime * Speed);
                }
                transform.Rotate(0, 0, zangle * direction * Time.deltaTime);
                ++count;

            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}