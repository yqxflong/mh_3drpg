using System;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.CLR.Method;
using EB.Sparx;

public class SparxAPIAdapter : CrossBindingAdaptor
{
    public override Type BaseCLRType
    {
        get
        {
            return typeof(SparxAPI);
        }
    }

    public override Type AdaptorType
    {
        get
        {
            return typeof(Adaptor);
        }
    }

    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    {
        return new Adaptor(appdomain, instance);
    }

    public class Adaptor : SparxAPI, CrossBindingAdaptorType
    {
        ILTypeInstance instance;
        ILRuntime.Runtime.Enviorment.AppDomain appdomain;

        public Adaptor()
        {

        }

        public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            this.appdomain = appdomain;
            this.instance = instance;
        }

        public ILTypeInstance ILInstance {
            get
            {
                return instance;
            }
        }

        public ILRuntime.Runtime.Enviorment.AppDomain AppDomain 
        { 
            get { return appdomain; } 
            set { appdomain = value; } 
        }

        public void SetILInstance(ILTypeInstance instance)
        {
            this.instance = instance;
        }

        IMethod mProcessResponseMethod;
        bool mProcessResponseMethodGot;
        public override bool ProcessResponse(Response response)
        {
            if (!mProcessResponseMethodGot)
            {
                mProcessResponseMethod = instance.Type.GetMethod("ProcessResponse", 1);
                mProcessResponseMethodGot = true;
            }

            if (mProcessResponseMethod != null)
            {
                return (bool)appdomain.Invoke(mProcessResponseMethod, instance, response);
            }

            return base.ProcessResponse(response);
        }

        IMethod mProcessErrorMethod;
        bool mProcessErrorMethodGot;
        public override bool ProcessError(Response response, eResponseCode errCode)
        {
            if (!mProcessErrorMethodGot)
            {
                mProcessErrorMethod = instance.Type.GetMethod("ProcessError", 2);
                mProcessErrorMethodGot = true;
            }

            if (mProcessErrorMethod != null)
            {
                return (bool)appdomain.Invoke(mProcessErrorMethod, instance, response, errCode);
            }

            return base.ProcessError(response, errCode);
        }

        IMethod mProcessResultMethod;
        bool mProcessResultMethodGot;
        protected override bool ProcessResult(Response response)
        {
            if (!mProcessResultMethodGot)
            {
                mProcessResultMethod = instance.Type.GetMethod("ProcessResult", 1);
                mProcessResultMethodGot = true;
            }

            if (mProcessResultMethod != null)
            {
                return (bool)appdomain.Invoke(mProcessResultMethod, instance, response);
            }

            return base.ProcessResult(response);
        }

        IMethod mGetMethod;
        bool mGetMethodGot;
        public new Request Get(string path)
        {
            if (!mGetMethodGot)
            {
                mGetMethod = instance.Type.GetMethod("Get", 1);
                mGetMethodGot = true;
            }

            if (mGetMethod != null)
            {
                return appdomain.Invoke(mGetMethod, instance, path) as Request;
            }

            return base.Get(path);
        }

        IMethod mPostMethod;
        bool mPostMethodGot;
        public new Request Post(string path)
        {
            if (!mPostMethodGot)
            {
                mPostMethod = instance.Type.GetMethod("Post", 1);
                mPostMethodGot = true;
            }

            if (mPostMethod != null)
            {
                return appdomain.Invoke(mPostMethod, instance, path) as Request;
            }

            return base.Post(path);
        }
    }
}
