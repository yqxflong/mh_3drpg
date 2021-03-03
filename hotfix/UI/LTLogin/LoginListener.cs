using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LoginListener
    {
        public event System.Action<EB.Sparx.Authenticator> ResolveAuthenticatorsEvent;
        public event System.Action<EB.Sparx.Authenticator[]> ResolveMultipleAuthenticatorsEvent;
        public event System.Action<EB.Sparx.Authenticator, Account> AuthorizedEvent;

        public void OnEnumerate(EB.Sparx.Authenticator[] authenticators)
        {
            if (authenticators.Length > 1)
            {
                if (ResolveMultipleAuthenticatorsEvent != null)
                {
                    ResolveMultipleAuthenticatorsEvent(authenticators);
                }
            }
            else
            {
                if (ResolveAuthenticatorsEvent != null)
                {
                    ResolveAuthenticatorsEvent(authenticators[0]);
                }
            }
        }
        
        public void OnAuthorized(EB.Sparx.Authenticator authenticator, Account account)
        {
            if (AuthorizedEvent != null)
            {
                AuthorizedEvent(authenticator, account);
            }
        }
        
    }
}