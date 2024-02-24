using System.Collections.Generic;
using SmartTutor.External.Future;
using SmartTutor.External.Promise;

public class GenericServices
{
    public static IPromise<T> Post<T>(string url, int timeout, int delay, Form form, Dictionary<string, string> headers)
    {
        return new Promise<T>((resolve, reject) =>
        {
            Request<T>.MakeRequest(url, timeout, delay, resolve, reject, Request<T>.Type.POST, true, form, null, headers);
        });
    }

    public static IPromise<T> Post<T>(string url, int timeout, int delay, byte[] param, Dictionary<string, string> headers)
    {
        return new Promise<T>((resolve, reject) =>
        {
            Request<T>.MakeRequest(url, timeout, delay, resolve, reject, Request<T>.Type.POST, false, null, param, headers);
        });
    }

    public static IPromise<T> Get<T>(string url, int timeout, int delay, Dictionary<string, string> headers)
    {
        return new Promise<T>((resolve, reject) =>
        {
            Request<T>.MakeRequest(url, timeout, delay, resolve, reject, Request<T>.Type.GET, false, null, null, headers);
        });
    }

    public static IPromise<T> Put<T>(string url, int timeout, int delay, byte[] param, Dictionary<string, string> headers)
    {
        return new Promise<T>((resolve, reject) =>
        {
            Request<T>.MakeRequest(url, timeout, delay, resolve, reject, Request<T>.Type.PUT, false, null, param, headers);
        });
    }

    public static IPromise<T> Delete<T>(string url, int timeout, int delay, Dictionary<string, string> headers)
    {
        return new Promise<T>((resolve, reject) =>
        {
            Request<T>.MakeRequest(url, timeout, delay, resolve, reject, Request<T>.Type.DELETE, false, null, null, headers);
        });
    }
}