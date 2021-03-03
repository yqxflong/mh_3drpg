#if USE_AMAZON
/* 
* Copyright 2014 Amazon.com,
* Inc. or its affiliates. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the
* "License"). You may not use this file except in compliance
* with the License. A copy of the License is located at
*
* http://aws.amazon.com/apache2.0/
*
* or in the "license" file accompanying this file. This file is
* distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, either express or implied. See the
* License for the specific language governing permissions and
* limitations under the License.
*/

namespace com.amazon.device.iap.cpt
{
    public class AmazonException : System.ApplicationException
    {
        public AmazonException() {}
        public AmazonException(string message) : base(message) {}
        public AmazonException(string message, System.Exception inner) : base(message, inner) {}
        protected AmazonException(System.Runtime.Serialization.SerializationInfo info,
                System.Runtime.Serialization.StreamingContext context) : base(info, context) {}
    }
}
#endif