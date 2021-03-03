//JohnySpline
//Author: Johny
//用于实现动态物体的平滑曲线运动
//===翻版必究===

using System.Collections.Generic;
using UnityEngine;

namespace Johny
{
        //*****  Spline  *****//
        public static class SplineUtils
        {
            public static float Sum(float l, float r)
            {
               return l + r;
            }

            public static Vector3 Sum(Vector3 l, Vector3 r){
                return l + r;
            }

            public static float Sub(float l, float r)
            {
                return l - r;
            }

            public static Vector3 Sub(Vector3 l, Vector3 r)
            {
                return l - r;
            }

            public static float Mul(float l, float r)
            {
                return l * r;
            }

            public static Vector3 Mul(Vector3 l, float r)
            {
                return l * r;
            }

            public static float SplineInterpolation(float x1, float x2, float r1, float r2, float t)
            {
                float dx = x1 - x2;
                float dx2 = dx + dx;
                float A = r1 + r2 + dx2;
                float B = r1 + dx + A;

                return x1 + t * (r1 + t * (A * t - B));
            }

            public static float GetGradient(float x1, float x2, float r1, float r2, float t)
            {
                float dx = 6.0f * (x1 - x2);
                float sr = r1 + r2;

                return r1 + t * (-2.0f * (r1 + sr) - dx + t * (3.0f * sr + dx));
            }

            public static Vector3 SplineInterpolation(Vector3 v1, Vector3 v2, Vector3 g1, Vector3 g2, float t)
            {
                return new Vector3(
                        SplineInterpolation(v1.x, v2.x, g1.x, g2.x, t),
                        SplineInterpolation(v1.y, v2.y, g1.y, g2.y, t),
                        SplineInterpolation(v1.z, v2.z, g1.z, g2.z, t)
                    );
            }

            public static Vector3 GetGradient(Vector3 v1, Vector3 v2, Vector3 g1, Vector3 g2, float t)
            {
                float retX = GetGradient(v1.x, v2.x, g1.x, g2.x, t);
                float retY = GetGradient(v1.y, v2.y, g1.y, g2.y, t);
                float retZ = GetGradient(v1.z, v2.z, g1.z, g2.z, t);
                return new Vector3(retX, retY, retZ);
            }
        }

        //*****  SplinePathFloat  *****//
        public class SplinePathFloat
        {
            public List<KeyValuePair<float, float>> _keys = new List<KeyValuePair<float, float>>();

            public SplinePathFloat(List<float> list = null)
            {
                if(list == null){
                    return;
                }
                
                for(int i = 0; i < list.Count;i++)
                {
                    var v = list[i];
                    _keys.Add(new KeyValuePair<float, float>(v, 0.0f));
                }
                CalculateGradient();
            }

            public void CalculateGradient(bool cycled = false)
            {
                if (_keys.Count == 0)
                    return;

                if (cycled)
                {
                    _keys.Add(_keys[0]);
                }

                if (cycled)
                {
                    float tmp = SplineUtils.Sub(_keys[1].Key, _keys[_keys.Count - 2].Key);
                    _keys[0] = new KeyValuePair<float, float>(_keys[0].Key, SplineUtils.Mul(tmp, 0.5f));
                    _keys[_keys.Count - 1] = new KeyValuePair<float, float>(_keys[_keys.Count - 1].Key, _keys[0].Value);
                }
                else
                {
                    if (_keys.Count > 1)
                    {
                        float tmp1 = SplineUtils.Sub(_keys[1].Key, _keys[0].Key);
                        float tmp2 = SplineUtils.Sub(_keys[_keys.Count - 1].Key, _keys[_keys.Count - 2].Key);
                        _keys[0] = new KeyValuePair<float, float>(_keys[0].Key, tmp1);
                        _keys[_keys.Count - 1] = new KeyValuePair<float,float>(_keys[_keys.Count - 1].Key, tmp2);
                    }
                }

                for (int i = 1; i < _keys.Count - 1; i++)
                {
                    float tmp = SplineUtils.Sub(_keys[i + 1].Key, _keys[i - 1].Key);
                    _keys[i] = new KeyValuePair<float,float>(_keys[i].Key, SplineUtils.Mul(tmp, 0.5f));
                }
            }

            public void Clear()
            {
                _keys.Clear();
            }

            public void addKey(float key)
            {
                _keys.Add(new KeyValuePair<float,float>(key, 0.0f));
            }

            public int GetKeysCount()
            {
                return _keys.Count;
            }

            public KeyValuePair<float, float> GetKey(int i)
            {
                return _keys[i];
            }

            public float getFrame(int sector, float t)
            {
                int i = sector;

                var v1 = _keys[i].Key;
                var v2 = _keys[i + 1].Key;
                var v3 = _keys[i].Value;
                var v4 = _keys[i + 1].Value;

                return SplineUtils.SplineInterpolation(v1, v2, v3, v4, t);
            }

            public float getGradient(int sector, float t)
            {
                int i = sector;

                var v1 = _keys[i].Key;
                var v2 = _keys[i + 1].Key;
                var v3 = _keys[i].Value;
                var v4 = _keys[i + 1].Value;

                return SplineUtils.GetGradient(v1, v2, v3, v4, t);
            }

            public float getGlobalFrame(float t)
            {
                if (t < 0.0f) t = 0.0f;
                if (t > 1.0f) t = 1.0f;

                int sectors = getSectors();
                if (sectors <= 0)
                    return 0.0f;

                float position = t * sectors;
                float tessSector = Mathf.Floor(position);

                if (tessSector >= sectors)
                    return _keys[_keys.Count - 1].Key;

                return getFrame((int)(tessSector), position - tessSector);
            }

            public float getGlobalGradient(float t)
            {
                if (t < 0.0f) t = 0.0f;
                if (t > 1.0f) t = 1.0f;

                int sectors = getSectors();
                if (sectors <= 0)
                    return 0.0f;

                float position = t * sectors;
                float tessSector = Mathf.Floor(position);

                if (tessSector >= sectors)
                    return _keys[_keys.Count - 1].Value;

                return getGradient((int)(tessSector), position - tessSector);
            }

            public int getSectors()
            {
                return _keys.Count - 1;
            }
        }

        //*****  SplinePathVector3  ******//
        public class SplinePathVector3
        {
            public List<KeyValuePair<Vector3, Vector3>> _keys = new List<KeyValuePair<Vector3, Vector3>>();

            public SplinePathVector3(List<Vector3> list = null)
            {
                if(list == null)
                {
                    return;
                }

                for(int i = 0; i < list.Count;i++)
                {
                    var v = list[i];
                    _keys.Add(new KeyValuePair<Vector3, Vector3>(v, Vector3.zero));
                }
                CalculateGradient();
            }

            public void CalculateGradient(bool cycled = false)
            {
                if (_keys.Count == 0)
                    return;

                if (cycled)
                {
                    _keys.Add(_keys[0]);
                }

                if (cycled)
                {
                    var tmp = SplineUtils.Sub(_keys[1].Key, _keys[_keys.Count - 2].Key);
                    _keys[0] = new KeyValuePair<Vector3, Vector3>(_keys[0].Key, SplineUtils.Mul(tmp, 0.5f));
                    _keys[_keys.Count - 1] = new KeyValuePair<Vector3, Vector3>(_keys[_keys.Count - 1].Key, _keys[0].Value);
                }
                else
                {
                    if (_keys.Count > 1)
                    {
                        var tmp1 = SplineUtils.Sub(_keys[1].Key, _keys[0].Key);
                        var tmp2 = SplineUtils.Sub(_keys[_keys.Count - 1].Key, _keys[_keys.Count - 2].Key);
                        _keys[0] = new KeyValuePair<Vector3, Vector3>(_keys[0].Key, tmp1);
                        _keys[_keys.Count - 1] = new KeyValuePair<Vector3,Vector3>(_keys[_keys.Count - 1].Key, tmp2);
                    }
                }

                for (int i = 1; i < _keys.Count - 1; i++)
                {
                    var tmp = SplineUtils.Sub(_keys[i + 1].Key, _keys[i - 1].Key);
                    _keys[i] = new KeyValuePair<Vector3,Vector3>(_keys[i].Key, SplineUtils.Mul(tmp, 0.5f));
                }
            }

            public void Clear()
            {
                _keys.Clear();
            }

            public void addKey(Vector3 key)
            {
                _keys.Add(new KeyValuePair<Vector3, Vector3>(key, Vector3.zero));
            }

            public int GetKeysCount()
            {
                return _keys.Count;
            }

            public KeyValuePair<Vector3, Vector3> GetKey(int i)
            {
                return _keys[i];
            }

            public Vector3 getFrame(int sector, float t)
            {
                int i = sector;

                var v1 = _keys[i].Key;
                var v2 = _keys[i + 1].Key;
                var v3 = _keys[i].Value;
                var v4 = _keys[i + 1].Value;

                return SplineUtils.SplineInterpolation(v1, v2, v3, v4, t);
            }

            public Vector3 getGradient(int sector, float t)
            {
                int i = sector;

                var v1 = _keys[i].Key;
                var v2 = _keys[i + 1].Key;
                var v3 = _keys[i].Value;
                var v4 = _keys[i + 1].Value;

                return SplineUtils.GetGradient(v1, v2, v3, v4, t);
            }

            public Vector3 getGlobalFrame(float t)
            {
                if (t < 0.0f) t = 0.0f;
                if (t > 1.0f) t = 1.0f;

                int sectors = getSectors();
                if (sectors <= 0)
                    return Vector3.zero;

                float position = t * sectors;
                float tessSector = Mathf.Floor(position);

                if (tessSector >= sectors)
                    return _keys[_keys.Count - 1].Key;

                return getFrame((int)(tessSector), position - tessSector);
            }

            public Vector3 getGlobalGradient(float t)
            {
                if (t < 0.0f) t = 0.0f;
                if (t > 1.0f) t = 1.0f;

                int sectors = getSectors();
                if (sectors <= 0)
                    return Vector3.zero;

                float position = t * sectors;
                float tessSector = Mathf.Floor(position);

                if (tessSector >= sectors)
                    return _keys[_keys.Count - 1].Value;

                return getGradient((int)(tessSector), position - tessSector);
            }

            public int getSectors()
            {
                return _keys.Count - 1;
            }
        }

        //*****  TimedSplineFloat  *****//
        public class TimedSplineFloat
        {
            public struct KeyFrame
            {
                public float value;
                public float td, ts;
                public float time;

                public KeyFrame(float key_, float time_)
                {
                    value = key_;
                    time = time_;
                    td = 0.0f;
                    ts = 0.0f;
                }
            };

            List<KeyFrame> _keys = new List<KeyFrame>();
            int _currentSector = 1;

            public TimedSplineFloat(List<KeyValuePair<float, float>> list = null)
            {
                if(list == null){
                    return;
                }

                for(int i = 0; i < list.Count;i++)
                {
                    var p = list[i];
                    _keys.Add(new KeyFrame(p.Value, p.Key));
                }

                CalculateGradient();
            }

            public void Clear()
            {
                _keys.Clear();
                _keys.Clear();
                _currentSector = 1;
            }

            public void CalculateGradient()
            {
                _keys.Sort((KeyFrame a, KeyFrame b) =>
                {
                    return a.time.CompareTo(b.time);
                });

                int e = _keys.Count - 1;

                var tmp_key0 = _keys[0];
                var tmp_keye = _keys[e];

                float tmp1 = SplineUtils.Sub(_keys[1].value, _keys[0].value);
                float tmp2 = SplineUtils.Sub(_keys[e].value, _keys[e - 1].value);

                tmp_key0.ts = SplineUtils.Mul(tmp1, 0.5f * (_keys[1].time - _keys[0].time));
                tmp_keye.td = SplineUtils.Mul(tmp2, 0.5f * (_keys[e].time - _keys[e - 1].time));

                _keys[0] = tmp_key0;
                _keys[e] = tmp_keye;

                for (int i = 1; i < e; i++)
                {
                    var tmp_keyi = _keys[i];
                    float tmp3 = SplineUtils.Sub(_keys[i + 1].value, _keys[i - 1].value);
                    tmp_keyi.ts = tmp_keyi.td = SplineUtils.Mul(tmp3, 0.5f);

                    float inv_dd = 1.0f / (_keys[i + 1].time - _keys[i - 1].time);

                    float k1 = 2.0f * (_keys[i].time - _keys[i - 1].time) * inv_dd;
                    float k2 = 2.0f * (_keys[i + 1].time - _keys[i].time) * inv_dd;

                    tmp_keyi.td = SplineUtils.Mul(tmp_keyi.td, k1);
                    tmp_keyi.ts = SplineUtils.Mul(tmp_keyi.ts, k2);

                    _keys[i] = tmp_keyi;
                }
            }

            public void addKey(float time, float key)
            {
                _keys.Add(new KeyFrame(key, time));
            }

            public int GetKeysCount()
            {
                return _keys.Count;
            }

            public float GetKey(int i)
            {
                return _keys[i].value;
            }

            public void SetKey(int i, float key)
            {
                var tmp_keyi = _keys[i];
                tmp_keyi.value = key;
                _keys[i] = tmp_keyi;
            }

            public float getGlobalFrame(float t)
            {
                if (_keys.Count != 0)
                {
                    float con1 = _keys[0].time - 0.0001f;
                    float con2 = _keys[_keys.Count - 1].time + 0.0001f;
                    if (!(t > con1 && t < con2))
                    {
                        EB.Debug.LogError("Spline=getGlobalFrame=t: " + t.ToString() + "=con1: " + con1.ToString() + "=con2: " + con2.ToString());
                    }
                }

                if (t < _keys[0].time + 0.0001f)
                {
                    _currentSector = 1;
                    return _keys[0].value;
                }
                else if (t > _keys[_keys.Count - 1].time - 0.0001f)
                {
                    _currentSector = 1;
                    return _keys[_keys.Count - 1].value;
                }

                int sector = getSector(t);
                float localTime = Mathf.Clamp((t - _keys[sector].time) / (_keys[sector + 1].time - _keys[sector].time), 0.0f, 1.0f);

                return getFrame(sector, localTime);
            }

            public float getGlobalGradient(float t)
            {
                int sector = getSector(t);
                float localTime = (t - _keys[sector].time) / (_keys[sector + 1].time - _keys[sector].time);
                return getGradient(sector, localTime);
            }

            public bool empty()
            {
                return _keys.Count == 0;
            }


            private int getSector(float t) 
            {
                const float EPS = 0.0001f;
                int size = _keys.Count;
                if (t < EPS) /* t <= 0 */
                    _currentSector = 1;

                while (_currentSector > 0 && _keys[_currentSector - 1].time >= t)
                    _currentSector--;

                while (_currentSector < size && _keys[_currentSector].time < t)

                    _currentSector++;

                if (_currentSector == 0 || _currentSector == size)
                    _currentSector = 1;


                return _currentSector - 1;
            }

            public float getFrame(int sector, float t) {
                var k1 = _keys[sector];
                var k2 = _keys[sector + 1];

                return SplineUtils.SplineInterpolation(k1.value, k2.value, k1.ts, k2.td, t);
            }

            public float getGradient(int sector, float t)
            {
                var k1 = _keys[sector];
                var k2 = _keys[sector + 1];
                return SplineUtils.GetGradient(k1.value, k2.value, k1.ts, k2.td, t);
            }
        }

         //*****  TimedSplineVector3  *****//
        public class TimedSplineVector3
        {
            public struct KeyFrame
            {
                public Vector3 value;
                public Vector3 td, ts;
                public float time;

                public KeyFrame(Vector3 key_, float time_)
                {
                    value = key_;
                    time = time_;
                    td = Vector3.zero;
                    ts = Vector3.zero;
                }
            };

            List<KeyFrame> _keys = new List<KeyFrame>();
            int _currentSector = 1;

            public TimedSplineVector3(List<KeyValuePair<float, Vector3>> list = null)
            {
                if(list == null){
                    return;
                }

                for(int i = 0; i < list.Count;i++)
                {
                    var p = list[i];
                    _keys.Add(new KeyFrame(p.Value, p.Key));
                }

                CalculateGradient();
            }

            public void Clear()
            {
                _keys.Clear();
                _keys.Clear();
                _currentSector = 1;
            }

            public void CalculateGradient()
            {
                _keys.Sort((KeyFrame a, KeyFrame b) =>
                {
                    return a.time.CompareTo(b.time);
                });

                int e = _keys.Count - 1;

                var tmp_key0 = _keys[0];
                var tmp_keye = _keys[e];

                var tmp1 = SplineUtils.Sub(_keys[1].value, _keys[0].value);
                var tmp2 = SplineUtils.Sub(_keys[e].value, _keys[e - 1].value);

                tmp_key0.ts = SplineUtils.Mul(tmp1, 0.5f * (_keys[1].time - _keys[0].time));
                tmp_keye.td = SplineUtils.Mul(tmp2, 0.5f * (_keys[e].time - _keys[e - 1].time));

                _keys[0] = tmp_key0;
                _keys[e] = tmp_keye;

                for (int i = 1; i < e; i++)
                {
                    var tmp_keyi = _keys[i];
                    var tmp3 = SplineUtils.Sub(_keys[i + 1].value, _keys[i - 1].value);
                    tmp_keyi.ts = tmp_keyi.td = SplineUtils.Mul(tmp3, 0.5f);

                    float inv_dd = 1.0f / (_keys[i + 1].time - _keys[i - 1].time);

                    float k1 = 2.0f * (_keys[i].time - _keys[i - 1].time) * inv_dd;
                    float k2 = 2.0f * (_keys[i + 1].time - _keys[i].time) * inv_dd;

                    tmp_keyi.td = SplineUtils.Mul(tmp_keyi.td, k1);
                    tmp_keyi.ts = SplineUtils.Mul(tmp_keyi.ts, k2);

                    _keys[i] = tmp_keyi;
                }
            }

            public void addKey(float time, Vector3 key)
            {
                _keys.Add(new KeyFrame(key, time));
            }

            public int GetKeysCount()
            {
                return _keys.Count;
            }

            public Vector3 GetKey(int i)
            {
                return _keys[i].value;
            }

            public void SetKey(int i, Vector3 key)
            {
                var tmp_keyi = _keys[i];
                tmp_keyi.value = key;
                _keys[i] = tmp_keyi;
            }

            public Vector3 getGlobalFrame(float t)
            {
                if (_keys.Count != 0)
                {
                    float con1 = _keys[0].time - 0.0001f;
                    float con2 = _keys[_keys.Count - 1].time + 0.0001f;
                    if (!(t > con1 && t < con2))
                    {
                        EB.Debug.LogError("Spline=getGlobalFrame=t: " + t.ToString() + "=con1: " + con1.ToString() + "=con2: " + con2.ToString());
                    }
                }

                if (t < _keys[0].time + 0.0001f)
                {
                    _currentSector = 1;
                    return _keys[0].value;
                }
                else if (t > _keys[_keys.Count - 1].time - 0.0001f)
                {
                    _currentSector = 1;
                    return _keys[_keys.Count - 1].value;
                }

                int sector = getSector(t);
                float localTime = Mathf.Clamp((t - _keys[sector].time) / (_keys[sector + 1].time - _keys[sector].time), 0.0f, 1.0f);

                return getFrame(sector, localTime);
            }

            public Vector3 getGlobalGradient(float t)
            {
                int sector = getSector(t);
                float localTime = (t - _keys[sector].time) / (_keys[sector + 1].time - _keys[sector].time);
                return getGradient(sector, localTime);
            }

            public bool empty()
            {
                return _keys.Count == 0;
            }


            private int getSector(float t) 
            {
                const float EPS = 0.0001f;
                int size = _keys.Count;
                if (t < EPS) /* t <= 0 */
                    _currentSector = 1;

                while (_currentSector > 0 && _keys[_currentSector - 1].time >= t)
                    _currentSector--;

                while (_currentSector < size && _keys[_currentSector].time < t)

                    _currentSector++;

                if (_currentSector == 0 || _currentSector == size)
                    _currentSector = 1;


                return _currentSector - 1;
            }

            public Vector3 getFrame(int sector, float t) {
                var k1 = _keys[sector];
                var k2 = _keys[sector + 1];

                return SplineUtils.SplineInterpolation(k1.value, k2.value, k1.ts, k2.td, t);
            }

            public Vector3 getGradient(int sector, float t)
            {
                var k1 = _keys[sector];
                var k2 = _keys[sector + 1];
                return SplineUtils.GetGradient(k1.value, k2.value, k1.ts, k2.td, t);
            }
        }
    }