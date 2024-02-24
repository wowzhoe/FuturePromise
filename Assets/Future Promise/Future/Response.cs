using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace SmartTutor.External.Future
{
    public class Response<T>
    {
        public Response() { }

        public Response(UnityWebRequestAsyncOperation uwr, Action<T> resolve, Action<Exception> reject)
        {
            if (uwr.webRequest.isNetworkError)
            {
                reject(new ApplicationException(uwr.webRequest.error));
            }
            else
            {
                Dictionary<Type, Action> isValidType = new Dictionary<Type, Action>
                {
                    { typeof(Texture2D), () => resolve((T) (object) DownloadHandlerTexture.GetContent(uwr.webRequest))},
                    { typeof(AudioClip), () => resolve((T) (object) DownloadHandlerAudioClip.GetContent(uwr.webRequest))},
                    { typeof(string), () => resolve((T) (object) uwr.webRequest.downloadHandler.text)},
                    { typeof(AssetBundle), () => resolve((T) (object) DownloadHandlerAssetBundle.GetContent(uwr.webRequest))},
                };
                isValidType[typeof(T)]();
            }
        }
    }
}
