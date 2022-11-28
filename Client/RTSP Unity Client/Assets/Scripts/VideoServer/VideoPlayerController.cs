using System;
using LibVLCSharp;
using UnityEngine;
using UnityEngine.UI;

namespace Arwel.Scripts.UI
{
	public class VideoPlayerController : MonoBehaviour
	{
		private string Path => $"rtsp://{VideoPath.Server}:{VideoPath.Port}/{VideoPath.Video}";

		public static LibVLC
			libVLC; //The LibVLC class is mainly used for making MediaPlayer and Media objects. You should only have one LibVLC instance.

		public MediaPlayer mediaPlayer; //MediaPlayer is the main class we use to interact with VLC

		//Screen
		public RawImage canvasScreen; //Assign a Canvas RawImage to render on a GUI object

		Texture2D _vlcTexture = null; //This is the texture libVLC writes to directly. It's private.
		public RenderTexture texture = null; //We copy it into this texture which we actually use in unity.

		public bool flipTextureX = false; //No particular reason you'd need this but it is sometimes useful
		public bool flipTextureY = true; //Set to false on Android, to true on Windows

		public AudioSource ambient;

		public bool automaticallyFlipOnAndroid = true; //Automatically invert Y on Android

		//Unity Awake, OnDestroy, and Update functions

		#region unity

		void Awake()
		{
			//Setup LibVLC
			if (libVLC == null)
				CreateLibVLC();

			if (canvasScreen == null)
				canvasScreen = GetComponent<RawImage>();

			//Automatically flip on android
			if (automaticallyFlipOnAndroid && Application.platform == RuntimePlatform.Android)
				flipTextureY = !flipTextureY;

			//Setup Media Player
			CreateMediaPlayer();
		}

		void OnDestroy()
		{
			//Dispose of mediaPlayer, or it will stay in nemory and keep playing audio
			DestroyMediaPlayer();
		}

		void Update()
		{
			//Get size every frame
			uint height = 0;
			uint width = 0;
			mediaPlayer?.Size(0, ref width, ref height);

			//Automatically resize output textures if size changes
			if (_vlcTexture == null || _vlcTexture.width != width || _vlcTexture.height != height)
			{
				ResizeOutputTextures(width, height);
			}

			if (_vlcTexture != null)
			{
				//Update the vlc texture (tex)
				var texptr = mediaPlayer.GetTexture(width, height, out bool updated);
				if (updated)
				{
					_vlcTexture.UpdateExternalTexture(texptr);

					//Copy the vlc texture into the output texture, flipped over
					var flip = new Vector2(flipTextureX ? -1 : 1, flipTextureY ? -1 : 1);
					Graphics.Blit(_vlcTexture, texture, flip,
						Vector2.zero); //If you wanted to do post processing outside of VLC you could use a shader here.
				}
			}

			if (mediaPlayer.IsPlaying)
			{
				if (mediaPlayer.Media.Statistics.DecodedAudio > 0) //detec audio
				{
					ambient.Stop();
				}
			}
			else
			{
				if (!ambient.isPlaying)
				{
					ambient.Play();
				}
			}
		}

		#endregion


		//Public functions that expose VLC MediaPlayer functions in a Unity-friendly way. You may want to add more of these.

		#region vlc

		public void Open()
		{
			Debug.Log(Path);
			if (mediaPlayer.Media != null)
				mediaPlayer.Media.Dispose();

			var trimmedPath = Path.Trim(new char[]
				{'"'}); //Windows likes to copy paths with quotes but Uri does not like to open them
			mediaPlayer.Media = new Media(new Uri(trimmedPath));
			Play();
		}

		public void Play()
		{
			mediaPlayer.Play();
		}

		public void Stop()
		{
			mediaPlayer?.Stop();

			_vlcTexture = null;
			texture = null;
		}

		//This returns the video orientation for the currently playing video, if there is one
		public VideoOrientation? GetVideoOrientation()
		{
			var tracks = mediaPlayer?.Tracks(TrackType.Video);

			if (tracks == null || tracks.Count == 0)
				return null;

			var orientation =
				tracks[0]?.Data.Video
					.Orientation; //At the moment we're assuming the track we're playing is the first track

			return orientation;
		}

		#endregion

		//Private functions create and destroy VLC objects and textures
		//Create a new static LibVLC instance and dispose of the old one. You should only ever have one LibVLC instance.
		void CreateLibVLC()
		{
			//Dispose of the old libVLC if necessary
			if (libVLC != null)
			{
				libVLC.Dispose();
				libVLC = null;
			}

			Core.Initialize(Application.dataPath); //Load VLC dlls
			libVLC = new LibVLC(
				enableDebugLogs: true); //You can customize LibVLC with advanced CLI options here https://wiki.videolan.org/VLC_command-line_help/

			//Setup Error Logging
			Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
		}

		//Create a new MediaPlayer object and dispose of the old one. 
		void CreateMediaPlayer()
		{
			if (mediaPlayer != null)
			{
				DestroyMediaPlayer();
			}

			mediaPlayer = new MediaPlayer(libVLC);
		}

		//Dispose of the MediaPlayer object. 
		void DestroyMediaPlayer()
		{
			mediaPlayer?.Stop();
			mediaPlayer?.Dispose();
			mediaPlayer = null;
		}

		//Resize the output textures to the size of the video
		void ResizeOutputTextures(uint px, uint py)
		{
			var texptr = mediaPlayer.GetTexture(px, py, out bool updated);
			if (px != 0 && py != 0 && updated && texptr != IntPtr.Zero)
			{
				//If the currently playing video uses the Bottom Right orientation, we have to do this to avoid stretching it.
				if (GetVideoOrientation() == VideoOrientation.BottomRight)
				{
					(px, py) = (py, px);
				}

				_vlcTexture =
					Texture2D.CreateExternalTexture((int) px, (int) py, TextureFormat.RGBA32, false, true,
						texptr); //Make a texture of the proper size for the video to output to
				texture = new RenderTexture(_vlcTexture.width, _vlcTexture.height, 0,
					RenderTextureFormat.ARGB32); //Make a renderTexture the same size as vlctex

				if (canvasScreen != null)
					canvasScreen.texture = texture;
			}
		}
	}
}