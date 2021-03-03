//JohnySpline
//Author: Johny
//用于实现动态物体的平滑曲线运动
//===翻版必究===

using System.Collections.Generic;
using UnityEngine;

namespace Johny
{
    public class SplineBezier
    {
        private struct Item
        {
            public Vector3 p1, p2, p3;
            public float lenght_begin;
            public float lenght_end;
            public Vector3 A, B, C;

            public Item(Vector3 pp1, Vector3  pp2, Vector3  pp3, ref float l0, ref float lenghtXY)
            {
                p1 = pp1;
                p2 = pp2;
                p3 = pp3;
                A = p1;
                B = 2.0f * (p2 - p1);
                C = (p1 - 2.0f * p2 + p3);
                lenght_begin = l0;
                lenght_end = 0.0f;

                float lenght_max = 0.0f;
                Vector3 pb = p1, pe = new Vector3();
                Vector3 peXY = pe, pbXY = pb;
                for (int i = 1; i <= 10; i++)
                {
                    pe = getFrame(i / 10.0f);
                    lenght_max += (pe - pb).magnitude;

                    peXY = pe; pbXY = pb;
                    peXY.z = 0; pbXY.z = 0;
                    lenghtXY += (peXY - pbXY).magnitude;
                    pb = pe;
                }
                lenght_end = lenght_begin + lenght_max;
                l0 = lenght_end;
            }

            public Vector3 getFrame(float time_bool)
            {
                return A + B * time_bool + C * time_bool * time_bool;
            }
        };

        public bool error;
        private int size, currentItem;
        private float lenght_max, lenght_max_xy;
        private List<Item> items = new List<Item>();
        public List<Vector3> keys = new List<Vector3>();
        

        public SplineBezier()
        {
            Clear();
        }

        public void Clear()
        {
            keys.Clear();
            items.Clear();
            size = 0;
            error = true;
            currentItem = 0;
            lenght_max = 0;
            lenght_max_xy = 0;
        }

        public void addKey(Vector3 newKey)
        {
            keys.Add(newKey);
            size++;
        }

        public bool CalculateGradient(bool cycled = false)
        {
            error = false;
            while (size < 3)
            { 
                keys.Add(new Vector3(0.0f, size * (-1.0f), 0));
                size++;
            }
            lenght_max_xy = 0.0f;
            for (int i = 1; i < size - 1; i++)
            {
                Vector3 key0, key1, key2;
                if (i == 1)
                {
                    key0 = keys[0];
                }
                else
                {
                    key0 = (keys[i - 1] + keys[i]) / 2.0f;
                }
                key1 = keys[i];
                if (i == (size - 2))
                {
                    key2 = keys[size - 1];
                }
                else
                {
                    key2 = (keys[i] + keys[i + 1]) / 2.0f;
                }
                items.Add(new Item(key0, key1, key2, ref lenght_max, ref lenght_max_xy));
            }

            return true;
        }

        public Vector3 getGlobalFrame(float time_bool)
        {
            if (error)
            {
                return new Vector3(0.0f, 0.0f, 0.0f);
            }
            time_bool = Mathf.Clamp(time_bool, 0.0f, 1.0f);

            float lenght_norm = lenght_max * time_bool;
            float lenght_b = items[currentItem].lenght_begin;
            while (lenght_norm < lenght_b)
            {
                currentItem--;
                lenght_b = items[currentItem].lenght_begin;
            }
            float lenght_e = items[currentItem].lenght_end;
            while (lenght_norm > lenght_e)
            {
                currentItem++;
                lenght_e = items[currentItem].lenght_end;
            }
            lenght_b = items[currentItem].lenght_begin;
            return items[currentItem].getFrame((lenght_norm - lenght_b) / (lenght_e - lenght_b));
        }

        public float GetLenght()
        {
            return lenght_max;
        }

        public float GetLenghtXY()
        {
            return lenght_max_xy;
        }

        public int GetKeysCount()
        {
            return keys.Count;
        }

        public SplinePathVector3 GetSplinePathFPoint(float dicret_mul = 1.0f)
        {
            SplinePathVector3 newPath = new SplinePathVector3();

            float fullLenght = 0.0f;
            Vector3 prevV3 = getGlobalFrame(0.0f);

            int STEPS_COUNT = (int)(GetKeysCount() * dicret_mul);
            for (int i = 1; i < STEPS_COUNT; ++i)
            {
                float t = (float)(i) / STEPS_COUNT;
                Vector3 p = getGlobalFrame(t);
                fullLenght += Vector3.Distance(prevV3, p);
                prevV3 = p;
            }

            prevV3 = getGlobalFrame(0.0f);
            newPath.addKey(new Vector3(prevV3.x, prevV3.y, prevV3.z));

            float dist = 0.0f;
            float dist_limmit = 0.0f;
            float STEP_BORDER = fullLenght / STEPS_COUNT;
            while (dist < fullLenght - STEP_BORDER / 2)
            {
                Vector3 p = getGlobalFrame(dist / fullLenght);
                dist_limmit += Vector3.Distance(p, prevV3);
                if (dist_limmit >= STEP_BORDER)
                {
                    newPath.addKey(new Vector3(p.x, p.y, p.z));
                    dist_limmit -= STEP_BORDER;
                }
                dist += STEP_BORDER / 10.0f;
                prevV3 = p;
            }

            prevV3 = getGlobalFrame(1.0f);
            newPath.addKey(new Vector3(prevV3.x, prevV3.y, prevV3.z));

            return newPath;
        }
    }
}

