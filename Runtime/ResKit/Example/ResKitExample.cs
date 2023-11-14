using Framework.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Example{
    
public class ResKitExample : MonoBehaviour {

        ResLoader loader;
        public AudioClip clip22;
        AudioPlayer player;
        //IEnumerator  Start()
        //{
          //  ResKit.Init(); 

           // loader = ResLoader.Allocate();

            //string clipNmae = @"D:\UnityProject\UnityProject_Git\ai\Assets\AssetBundleExport\Audio\1800_1001_Music_bgm.ogg";
            // loader.LoadLocalAudioAsync(new List<string>(){ clipNmae }, ()=> {

            //     Debug.Log("异步加载完成");
            // });

           // yield return new WaitForSeconds(2);
            //loader.LoadLocalAudioAsync(clipNmae, (b0, res) =>
            //{

            //    Debug.Log(res);
            //});
            //var clip=  loader.LoadLocalAudioSyncWhenAsyncFinish(clipNmae);
            //clip.LogInfo();

            //AB包加载
           // var sprit = loader.LoadSync<Sprite>("1800_1002_common_asset", "1800_0002_6001");

           // player = AudioKit.PlaySound(clip22, true, play => { Debug.LogError("Finish------"); });

            //
          //  var sprit1 = loader.LoadSync<Sprite>("resources/"+ "Path");

          //  player = AudioKit.PlaySound(clip22, true, play => { Debug.LogError("Finish------"); });

            //网络贴图
            //string url = @"http://pic.616pic.com/ys_b_img/00/44/76/IUJ3YQSjx1.jpg";
            //loader.Add2Load("NetImage:" + url, ResLoadType.NetImageRes,(boo, res) =>
            //{
            //    Texture tex = res.Asset.As<Texture>();
            //    FindObjectOfType<UnityEngine.UI.RawImage>().texture = tex;
            //});
            //loader.LoadAsync();

            //本地
            //LocalImage

            //string path = @"C:\Users\Public\Pictures";
            //loader.Add2Load("LocalImage:" + url, ResLoadType.LocalImageRes, (boo, res) =>
            //{
            //    Texture tex = res.Asset.As<Texture>();
            //    FindObjectOfType<UnityEngine.UI.RawImage>().texture = tex;
            //});
            //loader.LoadAsync();
       // }

        private void Start ()
        {
            loader = ResLoader.Allocate();

            string url = "http://downsc.chinaz.net/Files/DownLoad/sound1/201906/11582.mp3";
            loader.LoadNetAudioAsync(url, AudioType.MPEG, (bo, res) =>
            {
                if (bo)
                {
                    if (res != null && res.Asset)
                    {

                        AudioKit.PlayVoice((AudioClip)res.Asset);
                    }
                }
            });

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                loader.Recycle2Cache();
                loader = null;
            }
        }



        private void OnDestroy()
        {
            loader.Recycle2Cache();
            loader = null;
        }

    }
}