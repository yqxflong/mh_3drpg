using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using System.Reflection;


namespace Hotfix_LT.UI
{
    public static class GameUtils
    {
        public static UIEventTrigger GetUIEventTrigger(this GameObject go)
        {
            UIEventTrigger evtTrigger = go.GetComponent<UIEventTrigger>();

            if (evtTrigger == null)
            {
                evtTrigger = go.AddComponent<UIEventTrigger>();
            }

            if (evtTrigger == null)
            {
                EB.Debug.LogError("GetUIEventTrigger: Can't Add/Get UIEventTrigger!");
            }

            return evtTrigger;
        }

        // calculates an axis aligned bounding box to encapsulate all the points
        public static Bounds CalculateBounds(List<Vector3> allPoints)
        {
            Vector3 BoundingBoxMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 BoundingBoxMax = new Vector3(-float.MaxValue, -float.MaxValue, -float.MaxValue);

            for (int i = 0; i < allPoints.Count; ++i) // go over all points getting the min and max x,y,z elements
            {
                EncapsulatePoint(allPoints[i], ref BoundingBoxMin, ref BoundingBoxMax);
            }

            Vector3 BoundingBoxCenter = Vector3.Lerp(BoundingBoxMin, BoundingBoxMax, 0.5f);
            Vector3 BoundingBoxSize = BoundingBoxMax - BoundingBoxMin;

            return new Bounds(BoundingBoxCenter, BoundingBoxSize);
        }

        // calculates an axis aligned bounding box to encapsulate all the game objects
        public static Bounds CalculateBounds(List<GameObject> allPoints)
        {
            Vector3 BoundingBoxMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 BoundingBoxMax = new Vector3(-float.MaxValue, -float.MaxValue, -float.MaxValue);

            int count = allPoints.Count;
            for (int i = 0; i < count; ++i) // go over all points getting the min and max x,y,z elements
            {
                if (allPoints[i] == null)
                {
                    continue;
                }
                EncapsulatePoint(allPoints[i].transform.position, ref BoundingBoxMin, ref BoundingBoxMax);
            }

            Vector3 BoundingBoxCenter = Vector3.Lerp(BoundingBoxMin, BoundingBoxMax, 0.5f);
            Vector3 BoundingBoxSize = BoundingBoxMax - BoundingBoxMin;

            return new Bounds(BoundingBoxCenter, BoundingBoxSize);
        }

        public static Bounds CalculateBoundsWithOffset(List<GameObject> allPoints)
        {
            Vector3 BoundingBoxMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 BoundingBoxMax = new Vector3(-float.MaxValue, -float.MaxValue, -float.MaxValue);

            int count = allPoints.Count;
            for (int i = 0; i < count; ++i) // go over all points getting the min and max x,y,z elements
            {
                if (allPoints[i] == null)
                {
                    continue;
                }
                Vector3 pos = allPoints[i].transform.position;

                CapsuleCollider collider = allPoints[i].GetComponent<CapsuleCollider>();
                if (collider != null)
                {
                    pos = allPoints[i].transform.TransformPoint(collider.center);
                }

                EncapsulatePoint(pos, ref BoundingBoxMin, ref BoundingBoxMax);
            }

            Vector3 BoundingBoxCenter = Vector3.Lerp(BoundingBoxMin, BoundingBoxMax, 0.5f);
            Vector3 BoundingBoxSize = BoundingBoxMax - BoundingBoxMin;
            Bounds bound = new Bounds(BoundingBoxCenter, BoundingBoxSize);
            return bound;
        }

        public static Bounds CalculateBounds(GameObject[] allPoints)
        {
            Vector3 BoundingBoxMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 BoundingBoxMax = new Vector3(-float.MaxValue, -float.MaxValue, -float.MaxValue);

            int count = allPoints.Length;
            for (int i = 0; i < count; ++i) // go over all points getting the min and max x,y,z elements
            {
                EncapsulatePoint(allPoints[i].transform.position, ref BoundingBoxMin, ref BoundingBoxMax);
            }

            Vector3 BoundingBoxCenter = Vector3.Lerp(BoundingBoxMin, BoundingBoxMax, 0.5f);
            Vector3 BoundingBoxSize = BoundingBoxMax - BoundingBoxMin;

            return new Bounds(BoundingBoxCenter, BoundingBoxSize);
        }

        // get the character model for the local player 
        public static CharacterModel GetLocalPlayerCharacterModel()
        {
            PlayerController localController = PlayerManager.LocalPlayerController();
            if (null != localController)
            {
                return GetCharacterModel(localController.gameObject);
            }
            return null;
        }

        // get the character model 
        public static CharacterModel GetCharacterModel(GameObject characterObject)
        {
            if (null != characterObject)
            {
                CharacterComponent character = characterObject.GetComponent<CharacterComponent>();
                if (null != character)
                {
                    return character.Model;
                }
                //CharacterComponentPixie characterPixie = characterObject.GetComponent<CharacterComponentPixie>();
                //if (null != characterPixie)
                //{
                //    return characterPixie.model;
                //}
            }
            return null;
        }

        // Calculate gameObject render bounds
        public static bool CalculateRenderBounds(GameObject obj, ref Bounds combinedBounds)
        {
            bool isInitialized = false;
            combinedBounds.center = Vector3.zero;
            combinedBounds.size = Vector3.zero;
            if (null != obj)
            {
                if (obj.GetComponent<Renderer>() != null)
                {
                    combinedBounds.center = obj.GetComponent<Renderer>().bounds.center;
                    combinedBounds.size = obj.GetComponent<Renderer>().bounds.size;
                    isInitialized = true;
                }

                Renderer[] allRenderers = obj.GetComponentsInChildren<Renderer>();

                if (allRenderers != null)
                {
                    for (var i = 0; i < allRenderers.Length; i++)
                    {
                        Renderer subRenderer = allRenderers[i];

                        if ((subRenderer != obj.GetComponent<Renderer>()) && (null != subRenderer))
                        {
                            if (!isInitialized)
                            {
                                combinedBounds.center = subRenderer.bounds.center;
                                combinedBounds.size = subRenderer.bounds.size;
                                isInitialized = true;
                            }
                            else
                            {
                                combinedBounds.Encapsulate(subRenderer.bounds);
                            }
                        }
                    }
                }
            }
            return isInitialized;
        }

        // O(n2) algorithm - VERY VERY SLOW
        // combine the mesh with the other mesh and try and weld any verts in mesh to close enough verts in othermesh	
        // NOTE: only vertices and triangles are updated, everything else (UV's etc will be incorrect following this function)
        static public Mesh CombineMeshes(Mesh mesh, Matrix4x4 meshMat, Mesh otherMesh, Matrix4x4 otherMeshMat, float vertWeldThreshold)
        {
            if (null == mesh || null == otherMesh)
            {
                return mesh ? mesh : otherMesh;
            }

            List<Vector3> newVerts = new List<Vector3>(mesh.vertices);
            List<int> newTriangles = new List<int>(mesh.triangles);

            List<int> vertRemapping = new List<int>(otherMesh.vertices.Length);
            List<Vector3> newOtherMeshVerts = new List<Vector3>(otherMesh.vertices.Length);

            // go over all the verts initially setting them all to having not been remapped yet
            int totalCombinedVerts = mesh.vertices.Length + otherMesh.vertices.Length;
            for (int vert = mesh.vertices.Length; vert < totalCombinedVerts; ++vert)
            {
                vertRemapping.Add(vert);
                newOtherMeshVerts.Add(otherMeshMat.MultiplyPoint3x4(otherMesh.vertices[vert - mesh.vertices.Length]));
            }

            float thresholdSquared = vertWeldThreshold * vertWeldThreshold;
            // go over the verts, finding ones which can be welded together
            for (int vert = 0; vert < mesh.vertices.Length; ++vert)
            {
                newVerts[vert] = meshMat.MultiplyPoint3x4(mesh.vertices[vert]);

                for (int otherVert = 0; otherVert < otherMesh.vertices.Length; ++otherVert)
                {
                    // if this other vert has not been removed yet and it is close enough so that we can remove it
                    if (otherVert + mesh.vertices.Length == vertRemapping[otherVert] && (newVerts[vert] - newOtherMeshVerts[otherVert]).sqrMagnitude < thresholdSquared)
                    {
                        vertRemapping[otherVert] = vert;
                    }
                }
            }

            for (int vert = mesh.vertices.Length; vert < totalCombinedVerts; ++vert)
            {
                int index = vert - mesh.vertices.Length;
                if (vert != vertRemapping[index]) // if this vert has been removed
                {
                    continue;
                }
                newVerts.Add(newOtherMeshVerts[index]);
                vertRemapping[index] = newVerts.Count - 1;
            }

            for (int index = 0; index < otherMesh.triangles.Length; ++index)
            {
                newTriangles.Add(vertRemapping[otherMesh.triangles[index]]);
            }

            // Update mesh
            mesh.Clear();
            mesh.vertices = newVerts.ToArray();
            mesh.triangles = newTriangles.ToArray();
            mesh.RecalculateBounds();
            return mesh;
        }

        // see if the triangle intersects the bounds specified by minBounds and maxBounds (on the ground plane X,Z)
        public static bool IsTriangleInBoundsXZ(Vector3 triangleVert0, Vector3 triangleVert1, Vector3 triangleVert2, Vector3 minBounds, Vector3 maxBounds)
        {
            // look for a seperating axis
            if ((triangleVert0.x < minBounds.x && triangleVert1.x < minBounds.x && triangleVert2.x < minBounds.x) || // all triangle verts are less than the min.x value of the bounds

                (triangleVert0.x > maxBounds.x && triangleVert1.x > maxBounds.x && triangleVert2.x > maxBounds.x) || // all triangle verts are greater than the max.x value of the bounds

                (triangleVert0.z < minBounds.z && triangleVert1.z < minBounds.z && triangleVert2.z < minBounds.z) || // all triangle verts are less than the min.z value of the bounds

                (triangleVert0.z > maxBounds.z && triangleVert1.z > maxBounds.z && triangleVert2.z > maxBounds.z)) // all triangle verts are greater than the max.z value of the bounds
            {
                return false; // a seperating acis was found - out of bounds
            }
            return true;
        }

        // recursively fine the transform include inRoot and the whole hierarchy under the inRoot, this also include the object that is deactivate
        public static Transform SearchHierarchyForBone(Transform inRoot, string inName, bool ignoreDisabled = false)
        {
            if (inRoot == null || inName.Length <= 0)
            {
                return null;
            }
            // check if the current bone is the bone we're looking for, if so return it
            if (inRoot.name.Equals(inName))
            {
                return inRoot;
            }

            EB.Collections.Queue<Transform> queue = new EB.Collections.Queue<Transform>(16);
            Transform result = null;
            queue.Enqueue(inRoot);
            while (queue.Count > 0)
            {
                Transform it = queue.Dequeue();
                result = it.Find(inName);
                if (result && (!ignoreDisabled || result.gameObject.activeInHierarchy))
                {
                    return result;
                }

                int childCount = it.childCount;
                for (int i = 0; i < childCount; ++i)
                {
                    queue.Enqueue(it.GetChild(i));
                }
            }
            return null;
        }

        private static Transform InternalRecursive(Transform inRoot, string inName)
        {
            Transform result = inRoot.Find(inName);
            if (result)
            {
                return result;
            }

            // search through child bones for the bone we're looking for
            int childCount = inRoot.childCount;
            for (int i = 0; i < childCount; ++i)
            {
                // the recursive step; repeat the search one step deeper in the hierarchy
                result = InternalRecursive(inRoot.GetChild(i), inName);

                // a transform was returned by the search above that is not null,
                // it must be the bone we're looking for
                if (result)
                {
                    return result;
                }
            }
            return null;
        }

        public static void SetShaderRecursive(Transform parent, string shaderName)
        {
            Shader shader = Shader.Find(shaderName);
            if (shader == null)
            {
                EB.Debug.LogWarning("GameUtils::SetShaderRecursive can't find shader '{0}'!" , shaderName);
            }

            if (parent.GetComponent<Renderer>())
            {
                parent.GetComponent<Renderer>().material.shader = shader;
            }

            if (parent.childCount > 0)
            {
                for (var i = 0; i < parent.childCount; i++)
                {
                    var child = parent.GetChild(i);

                    if (child != null)
                    {
                        SetShaderRecursive(child, shaderName);
                    }
                }
            }
        }

        // pick a random enum value given an arbitrary offset value
        public static T GetRandomEnum<T>(int offset = 0)
        {
            System.Array A = System.Enum.GetValues(typeof(T));

            T V = (T)A.GetValue(UnityEngine.Random.Range(offset, A.Length));

            return V;
        }

        // calculate the forward vector of the quaternion
        public static Vector3 CalculateForward(Quaternion quat)
        {
            Matrix4x4 transformMat = Matrix4x4.TRS(Vector3.zero, quat, Vector3.one);
            const int ForwardColumn = 2;
            Vector3 forward = transformMat.GetColumn(ForwardColumn);
            return forward;
        }

        // normalize a quaternion (as of writing, Quaternion has no normalize function)
        public static void Normalize(ref Quaternion quat)
        {
            quat = Quaternion.Lerp(quat, quat, 1f); // Quaternion.Lerp will normalize the result after doing the lerp
        }

        public static Vector3 SubXZ(Vector3 v1, Vector3 v2)
        {
            Vector3 v = v1 - v2;
            v.y = 0f;
            return v;
        }

        public static float GetDistSqXZ(Vector3 v1, Vector3 v2)
        {
            Vector3 v = v2 - v1;
            v.y = 0;
            return v.sqrMagnitude;
        }

        public static float GetDistXZ(Vector3 v1, Vector3 v2)
        {
            Vector3 v = v2 - v1;
            v.y = 0;
            return v.magnitude;
        }

        public static float DotXZ(Vector3 v1, Vector3 v2)
        {
            return (v1.x * v2.x) + (v1.z * v2.z);
        }

        public static float Square(float input)
        {
            return input * input;
        }

        // remove y element, normalize vector, return true/false based on whether original vecor was large enough to be normalized
        public static bool NormalizeXZ(ref Vector3 toNormalize)
        {
            toNormalize.y = 0f;
            toNormalize.Normalize();
            const float TolSqr = 0.5f * 0.5f;
            return (toNormalize.sqrMagnitude > TolSqr); // if the normalize succeeded, the length of the resultant vector will be 1, else length 0f
        }

        // traverse upwards in the scene hierarchy, return first matching component 
        public static Component FindFirstComponentUpwards<T>(Transform current)
        {
            Component component = null;
            while (current != null)
            {
                component = current.GetComponent(typeof(T));
                if (component != null)
                    return component;

                current = current.parent;
            }
            return component;
        }

        public static bool FastRemove<T>(List<T> list, T element)
        {
            int index = list.IndexOf(element);
            if (index != -1)
            {
                FastRemove(list, index);
                return true;
            }
            return false;
        }

        public static void FastRemove<T>(List<T> list, int index)
        {
            if (index < list.Count)
            {
                int lastIndex = list.Count - 1;
                if (list.Count > 1)
                {
                    T lastItem = list[lastIndex];
                    list[index] = lastItem;
                }
                list.RemoveAt(lastIndex);
            }
        }

        public static string ConvertToFriendlyName(string codeName)
        {
            if (codeName == "")
            {
                return "";
            }

            string friendlyName = System.Char.ToUpper(codeName[0]) + "";
            int lastSplit = 1;
            for (int i = 1; i < codeName.Length; i++)
            {
                if (System.Char.IsUpper(codeName[i]))
                {
                    friendlyName += codeName.Substring(lastSplit, i - lastSplit) + " ";
                    lastSplit = i;
                }
            }

            return friendlyName + codeName.Substring(lastSplit);
        }

        // if the ground height cannot be found, this function will return the y value of the passed in pos
        public static float CalculateGroundHeight(Vector3 pos)
        {
            float result = 0f;
            RaycastHit hit;
            int mask = 1 << LayerMask.NameToLayer("Obstacle") | 1 << LayerMask.NameToLayer("Ground");
            CalculateGroundHeight(pos, mask, out hit, ref result);
            return result;
        }

        // if the ground height cannot be found, this function will set outGroundHeight to the y value of the passed in pos
        public static bool CalculateGroundHeight(Vector3 pos, int mask, out RaycastHit hit, ref float outGroundHeight)
        {
            outGroundHeight = pos.y;

            const float SphereCastStartYOffset = 5f; // this needs to be large enough that if a character is going up stairs, the start postion of the sphere case will be above the ground 
                                                     // but not too large that the start position may be above a higher level of ground like a bridge overhead
            const float SphereCastRadius = 0.35f; // this value ensures the sphere cast does not begin intersecting the ground, but is large enough to not go through seems
            const float GroundClearence = 0.1f; // how much we raise our returned hit point, so things aren't exactly on the ground
            const float SphereCastLength = 10f; // this needs to be long enough to find ground below 'pos'

            if (Physics.SphereCast(pos + (Vector3.up * SphereCastStartYOffset), SphereCastRadius, Vector3.down, out hit, SphereCastLength, mask))
            {
                outGroundHeight = hit.point.y + GroundClearence;
                return true;
            }
            else if (null != AstarPath.active) // the sphere cast should never fail, let's see if starting the sphere cast from the height of the nav mesh makes a difference
            {
                NNInfo info = AstarPath.active.GetNearest(pos);
                if (null != info.node)
                {
                    pos.y = info.clampedPosition.y;
                    if (Physics.SphereCast(pos + (Vector3.up * SphereCastStartYOffset), SphereCastRadius, Vector3.down, out hit, SphereCastLength, mask))
                    {
                        outGroundHeight = hit.point.y + GroundClearence;
                        return true;
                    }
                }
            }
            return false;
        }

        public static void UpdateUIVisiblity(Transform parent, string name, bool visible)
        {
            Transform t = SearchHierarchyForBone(parent, name);
            if (t != null)
            {
                NGUITools.SetActive(t.gameObject, visible);
            }
        }

        public static void SetDefaultLighting()
        {
            GameObject directionalLightGO = GameObject.Find("Main Light");
            if (directionalLightGO != null)
            {
                Light directionalLight = directionalLightGO.GetComponent<Light>();

                if (directionalLight != null)
                {
                    directionalLight.transform.position = GameVars.MainLightPosition;
                    directionalLight.transform.rotation = GameVars.MainLightRotation;
                    directionalLight.intensity = GameVars.MainLightIntensity;
                    directionalLight.color = GameVars.MainLightColor;
                }
            }

            UnityEngine.RenderSettings.ambientLight = GameVars.GlobalAmbient;
        }

        public static void ChangeChildrenLayers(GameObject parentObject, string layerName)
        {
            int layer = LayerMask.NameToLayer(layerName);
            ChangeChildrenLayers(parentObject, layer);
        }

        public static void ChangeChildrenLayers(GameObject parentObject, int layer)
        {
            Transform[] allChildren = parentObject.GetComponentsInChildren<Transform>(true);

            if (allChildren != null)
            {
                for (var i = 0; i < allChildren.Length; i++)
                {
                    allChildren[i].gameObject.layer = layer;
                }
            }
        }

        public static void ChangeBtnSprite(UIImageButton btn, string keyword)
        {
            btn.normalSprite = "Button_" + keyword + "_Up";
            btn.hoverSprite = "Button_" + keyword + "_Down";
            btn.pressedSprite = "Button_" + keyword + "_Down";
            btn.GetComponentInChildren<UISprite>().spriteName = "Button_" + keyword + "_Up";
        }

        public static void UpdateImageButtonSprite(GameObject target, string color)
        {
            target.GetComponent<UISprite>().spriteName = "Button_Square_" + color + "_Up";
            target.GetComponent<UIImageButton>().normalSprite = "Button_Square_" + color + "_Up";
            target.GetComponent<UIImageButton>().hoverSprite = "Button_Square_" + color + "_Down";
            target.GetComponent<UIImageButton>().pressedSprite = "Button_Square_" + color + "_Down";
        }

        // increase min/max to encapsulate the passed in point (utility function used by CalculateBounds)
        private static void EncapsulatePoint(Vector3 point, ref Vector3 BoundingBoxMin, ref Vector3 BoundingBoxMax)
        {
            BoundingBoxMin.x = point.x < BoundingBoxMin.x ? point.x : BoundingBoxMin.x;
            BoundingBoxMax.x = point.x > BoundingBoxMax.x ? point.x : BoundingBoxMax.x;

            BoundingBoxMin.y = point.y < BoundingBoxMin.y ? point.y : BoundingBoxMin.y;
            BoundingBoxMax.y = point.y > BoundingBoxMax.y ? point.y : BoundingBoxMax.y;

            BoundingBoxMin.z = point.z < BoundingBoxMin.z ? point.z : BoundingBoxMin.z;
            BoundingBoxMax.z = point.z > BoundingBoxMax.z ? point.z : BoundingBoxMax.z;
        }

        public static void SetShaderDefaults()
        {
            Vector4 fogColor = new Vector4(1.0f, 1.0f, 1.0f, 0.0f);
            Shader.SetGlobalColor("_FogColor", fogColor);
        }

        public static string FormatBytes(long bytes)
        {
            const int scale = 1024;
            string[] orders = new string[] { "GB", "MB", "KB", "Bytes" };
            long max = (long)Mathf.Pow(scale, orders.Length - 1);

            for (var i = 0; i < orders.Length; i++)
            {
                if (bytes > max)
                {
                    return string.Format("{0:##.##} {1}", decimal.Divide(bytes, max), orders[i]);
                }

                max /= scale;
            }

            return "0 Bytes";
        }

        public static T GetCopyOf<T>(this Component comp, T other) where T : Component
        {
            System.Type type = comp.GetType();
            if (type != other.GetType()) return null; // type mis-match
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
            PropertyInfo[] pinfos = type.GetProperties(flags);

            if (pinfos != null)
            {
                for (var i = 0; i < pinfos.Length; i++)
                {
                    PropertyInfo pinfo = pinfos[i];

                    if (pinfo.CanWrite)
                    {
                        try
                        {
                            pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                        }
                        catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
                    }
                }
            }

            FieldInfo[] finfos = type.GetFields(flags);

            if (finfos != null)
            {
                for (var i = 0; i < finfos.Length; i++)
                {
                    var finfo = finfos[i];
                    finfo.SetValue(comp, finfo.GetValue(other));
                }
            }
            return comp as T;
        }

        public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
        {
            return go.AddComponent<T>().GetCopyOf(toAdd) as T;
        }

        static public string LineFeed(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            str.Trim();
            string tempStr = "";

            for (var i = 0; i < str.Length; i++)
            {
                tempStr += str[i] + "\n";
            }

            return tempStr.Substring(0, tempStr.Length - 1);
        }

        static public int GetStringWidth(string str)
        {
            char[] chars = str.ToCharArray();
            int width = 0;

            for (var i = 0; i < chars.Length; i++)
            {
                var ch = chars[i];
                // see: http://php.net/manual/zh/function.mb-strwidth.php
                if (ch >= '\u0000' && ch <= '\u0019')
                {
                    width += 0;
                }
                else if (ch >= '\u0020' && ch <= '\u1fff')
                {
                    width += 1;
                }
                else if (ch >= '\u2000' && ch <= '\uff60')
                {
                    width += 2;
                }
                else if (ch >= '\uff61' && ch <= '\uff9f')
                {
                    width += 1;
                }
                else
                {
                    width += 2;
                }
            }

            return width;
        }


        static public GameObject InstantiateEx(Transform target, Transform parent, string name = "Clone")
        {
            GameObject go = GameObject.Instantiate(target.gameObject) as GameObject;
            go.name = name;
            go.transform.SetParent(parent, false);
            if (false == go.activeSelf) go.gameObject.SetActive(true);
            return go;
        }

        static public T InstantiateEx<T>(T target, Transform parent, string name = "Clone") where T : Component
        {
            Component com = target as Component;
            GameObject go = InstantiateEx(com.transform, parent, name);
            return go.GetComponent<T>();
        }

        static public List<LTShowItemData> ParseAwardArr(ArrayList arr)
        {
            if (arr == null)
            {
                return new List<LTShowItemData>();
            }

            List<LTShowItemData> items = new List<LTShowItemData>();
            for (var i = 0; i < arr.Count; i++)
            {
                var a = arr[i];
                string type = EB.Dot.String("type", a, string.Empty);
                if (string.IsNullOrEmpty(type))
                    type = EB.Dot.String("t", a, string.Empty);

                string id = EB.Dot.String("data", a, string.Empty);
                if (string.IsNullOrEmpty(id))
                    id = EB.Dot.String("n", a, string.Empty);

                int count = EB.Dot.Integer("quantity", a, 0);
                if (count <= 0)
                    count = EB.Dot.Integer("q", a, 0);
                items.Add(new LTShowItemData(id, count, type, false));
            }

            List<LTShowItemData> itemsnew = new List<LTShowItemData>();
            for (var i = 0; i < items.Count; i++)
            {
                MergeAdd(itemsnew, items[i]);
            }
            return itemsnew;
        }

        private static bool MergeAdd(List<LTShowItemData> itemsnew, LTShowItemData itemData)
        {
            for (var i = 0; i < itemsnew.Count; i++)
            {
                var v = itemsnew[i];

                if (v.id == itemData.id)
                {
                    v.count += itemData.count;
                    return true;
                }
            }
            itemsnew.Add(itemData);
            return false;
        }

        /// <summary>
        /// �����Ʒ�����Ϣ����Ϣ�����ñ�guide - words ��type����չʾ�������11Ϊϵͳ��Ϣ��
        /// </summary>
        /// <param name="itemDataArr"></param>
        static public void ShowAwardMsg(LTShowItemData[] itemDataArr)
        {
            if (itemDataArr == null) return;
            for (int i = 0; i < itemDataArr.Length; ++i)
            {
                ShowAwardMsg(itemDataArr[i]);
            }
        }
        //ͬ��
        static public void ShowAwardMsg(List<LTShowItemData> itemDataList)
        {
            itemDataList.ForEach(item => ShowAwardMsg(item));
        }
        //ͬ��
        static public void ShowAwardMsg(LTShowItemData itemData)
        {
            Hashtable data = Johny.HashtablePool.Claim();
            if (itemData.id.Equals(BalanceResourceUtil.GoldName))
            {
                data.Add("0", itemData.count);
                MessageTemplateManager.ShowMessage(901033, data, null);
            }
            else if (itemData.id.Equals(BalanceResourceUtil.HcName))
            {
                data.Add("0", itemData.count);
                MessageTemplateManager.ShowMessage(901034, data, null);
            }
            else
            {
                string resName = LTItemInfoTool.GetInfo(itemData.id, itemData.type,true).name;
                data.Add("0", resName);
                data.Add("1", itemData.count);
                MessageTemplateManager.ShowMessage(901032, data, null);
            }
            Johny.HashtablePool.Release(data);
            data = null;
        }

        /// <summary>
        /// zwr���޸ģ���Ʒ���ʱ����������11ϵͳ��Ϣ����ҪƮ�ֻ�����
        /// </summary>
        /// <param name="itemDataList"></param>
        static public void ShowAwardMsgOnlySys(List<LTShowItemData> itemDataList)
        {
            itemDataList.ForEach(item => ShowAwardMsgOnlySys(item));
        }
        //ͬ��
        static public void ShowAwardMsgOnlySys(LTShowItemData itemData)
        {
            Hashtable data = Johny.HashtablePool.Claim();
            if (itemData.id.Equals(BalanceResourceUtil.GoldName))
            {
                List<MessageTemplate> MT = MessageTemplateManager.Instance.GetMessageTemplate(901033);
                for (int i = 0; i < MT.Count; ++i)
                {
                    if (MT[i] is IMSystemMessage)
                    {
                        data.Add("0", itemData.count);
                        MT[i].ShowMessage(data, null);
                    }
                }
            }
            else if (itemData.id.Equals(BalanceResourceUtil.HcName))
            {
                List<MessageTemplate> MT = MessageTemplateManager.Instance.GetMessageTemplate(901034);
                for (int i = 0; i < MT.Count; i++)
                {
                    if (MT[i] is IMSystemMessage)
                    {
                        data.Add("0", itemData.count);
                        MT[i].ShowMessage(data, null);
                    }
                }
            }
            else
            {
                List<MessageTemplate> MT = MessageTemplateManager.Instance.GetMessageTemplate(901032);
                for (int i = 0; i < MT.Count; i++)
                {
                    if (MT[i] is IMSystemMessage)
                    {
                        string resName = LTItemInfoTool.GetInfo(itemData.id, itemData.type,true).name;
                        data.Add("0", resName);
                        data.Add("1", itemData.count);
                        MT[i].ShowMessage(data, null);
                    }
                }
            }
            Johny.HashtablePool.ReleaseRecursion(data);
            data = null;
        }

        public static Camera GetMainCamera()
        {
            var cam = Camera.main;
            if (cam == null)
            {
                GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
                if (mainCamera != null)
                {
                    cam = mainCamera.GetComponent<Camera>();
                }
            }
            if (cam == null)
            {
                GameObject mainCamera = GameObject.Find("Main Camera");
                if (mainCamera != null)
                {
                    cam = mainCamera.GetComponent<Camera>();
                }
            }
            return cam;
        }

        private static object _mainCameraHandle = null;
        private static int _originalCullingMask = -0xff;
        private static string[] _needOpens = new string[] { "UI" };
        public static void SetMainCameraActive(bool active)
        {
            //return;
            /*EB.Coroutines.ClearTimeout(_mainCameraHandle);
			Camera camera = GetMainCamera();
			if(camera ==null)
			{
				return;
			}

			if(_originalCullingMask== -0xff)
			{
				_originalCullingMask = camera.cullingMask;
			}

			if (active)
			{
				camera.cullingMask = _originalCullingMask;
			}
			else
			{
				int useCullingMask = 0;
				for(int i=0;i< _needOpens.Length;i++)
				{
					useCullingMask += 1 << LayerMask.NameToLayer(_needOpens[i]);
				}
				_mainCameraHandle = EB.Coroutines.SetTimeout(delegate () { if (camera != null) camera.cullingMask = useCullingMask; }, 100);
			}*/

            EB.Coroutines.ClearTimeout(_mainCameraHandle);

            Camera camera = GetMainCamera();
            if (camera != null)
            {
                if (active)
                {
                    camera.enabled = active;
                }
                else
                {
                    _mainCameraHandle = EB.Coroutines.SetTimeout(delegate () { if (camera != null) camera.enabled = active; }, 100);
                }
            }
        }

        public static string ColorToWebColor(Color c)
        {
            string ret = "#";
            for (int i = 0; i < 4; i++)
            {
                string t = Mathf.FloorToInt(c[i] * 255).ToString("x");
                if (t.Length < 2)
                {
                    t = "0" + t;
                }
                ret += t;
            }
            return ret;
        }

        public static string ColoredString(string s, Color c)
        {
            return "<color=" + ColorToWebColor(c) + ">" + s + "</color>";
        }

        /// <summary>
        /// 处理NGUI动态适配localposition数值2730*1536
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Vector2 GetPosWithConstrained(int x,int y)
        {
            if ((float)Screen.width / Screen.height > 1.78)
                return new Vector2((Screen.width*1536.0f / (2730*Screen.height)) * x, y );
            else
                return new Vector2(x, (Screen.height*2730.0f / (Screen.width*1536)) * y);
        }
        
        public static eAttrTabType ParseTabType(string str)
        {
            if (str.Contains("All"))
                return eAttrTabType.All;
            else if (str.Contains("Feng"))
                return eAttrTabType.Feng;
            else if (str.Contains("Huo"))
                return eAttrTabType.Huo;
            else if (str.Contains("Shui"))
                return eAttrTabType.Shui;
            EB.Debug.LogError("ParseTabType error str={0}", str);
            return eAttrTabType.All;
        }
        
        public static int FindComponentListIndex<T>(List<T> list,T t) where T:Component
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i]==t)
                {
                    return i;
                }
            }
            return 0;
        }

        public static string ApplyNumFormat(int data, bool isFormat)
        {
            if (!isFormat)
            {
                return data.ToString();
            }
            if (data >= 1000000000)
            {
                string str = string.Format("{0}G", data / 1000000000);
                return str;
            }
            else if (data >= 1000000)
            {
                string str = string.Format("{0}M", data / 1000000);
                return str;
            }
            else if (data >= 1000)
            {
                string str = string.Format("{0}K", data / 1000);
                return str;
            }
            else
            {
                return data.ToString();
            }
        }
    }
}