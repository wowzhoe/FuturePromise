using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using SmartTutor.External.Future.Utils;


namespace SmartTutor.External.Future
{
    public class Request<T>
    {
        public enum Type
        {
            GET = 0,
            PUT = 1,
            POST = 2,
            DELETE = 3
        }

        private static readonly Dictionary<Type, string>Types = new Dictionary<Type, string>
            {
                    { Type.GET, UnityWebRequest.kHttpVerbGET},
                    { Type.PUT, UnityWebRequest.kHttpVerbPUT},
                    { Type.POST, UnityWebRequest.kHttpVerbPOST},
                    { Type.DELETE, UnityWebRequest.kHttpVerbDELETE},
            };

        private static string GetType(Type type)
        {
            return Types.Keys.FirstOrDefault(s => s.Equals(type)).ToString();
        }

        public static async void MakeRequest(string url, int timeout, int delay, Action<T> resolve, Action<Exception> reject, Type type, bool useForm = false, Form form = null, byte[] param = null, Dictionary<string, string> headers = null)
        {
            UnityWebRequestAsyncOperation uwr = RequestAsync(url, timeout, type, useForm, form, param, headers);
            int time = delay * 1000;
            await Task.Delay(time);
            await uwr;
            Response<T> r = new Response<T>(uwr, resolve, reject);
        }

        private static UnityWebRequestAsyncOperation RequestAsync(string url, int timeout, Type type, bool useForm = false, Form form = null, byte[] param = null, Dictionary<string, string> headers = null)
        {
            UnityWebRequest uwr = new UnityWebRequest(url, GetType(type));
            uwr.timeout = timeout;
            uwr.downloadHandler = SetDownloadHandlerType(uwr, url);
            headers?.Each(s => uwr.SetRequestHeader(s.Key, s.Value));
            byte[] raw = useForm ? form?.form.data : param;
            UploadHandlerRaw uH = new UploadHandlerRaw(raw);
            uwr.uploadHandler = uH;
            UnityWebRequestAsyncOperation op = uwr.SendWebRequest();
            return op;
        }

        private static DownloadHandler SetDownloadHandlerType(UnityWebRequest uwr, string url)
        {
            Dictionary<System.Type, Action> isValidHandlersType = new Dictionary<System.Type, Action>
            {
                    { typeof(Texture2D), () => uwr.downloadHandler = new DownloadHandlerTexture()},
                    { typeof(AudioClip), () => uwr.downloadHandler = new DownloadHandlerAudioClip(url, AudioType.WAV)},
                    { typeof(string), () => uwr.downloadHandler = new DownloadHandlerBuffer()},
                    { typeof(AssetBundle), () => uwr.downloadHandler = new DownloadHandlerAssetBundle(url, 0)},
            };
            isValidHandlersType[typeof(T)]();
            return uwr.downloadHandler;
        }
    }
}