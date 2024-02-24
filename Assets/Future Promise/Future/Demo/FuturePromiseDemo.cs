using UnityEngine;
using SmartTutor.External.Future;

public class FuturePromiseDemo : MonoBehaviour
{
    public string imageUrl = "https://firebasestorage.googleapis.com/v0/b/skyward3d-5dc6c.appspot.com/o/crop%2Fcopier.jpeg?alt=media&token=c1ce9a17-72a5-4242-8699-a28373bc1b07"; //"http://manage.mediaplay.tv/media/videos/2/big/the-shining-official-trailer-hd.jpg";
    public int timeout = 10;
    public int delay = 5; // in milliseconds

    void Start()
    {
        // without timeout
       GenericServices.Get<Texture2D>(imageUrl, 0, 0, null)
           .Then(Result)
           .Catch(ex => Debug.LogException(ex, this));

        //        // with timeout
        //        Future<Texture2D>.Get(imageUrl, timeout, 0)
        //            .Then(value => Result(value))
        //            .Catch(ex => Debug.LogException(ex, this));
        //
        //        // without timeout but with delay
        //        Future<Texture2D>.Get(imageUrl, 0, delay)
        //            .Then(value => Result(value))
        //            .Catch(ex => Debug.LogException(ex, this));
    }

    public void Result(Texture2D texture)
    {
        GameObject imageHolder = new GameObject("image sample");
        SpriteRenderer source = imageHolder.AddComponent<SpriteRenderer>();
        Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        source.sprite = sprite;
        Debug.Log("image loaded succesfully!");
    }
}
