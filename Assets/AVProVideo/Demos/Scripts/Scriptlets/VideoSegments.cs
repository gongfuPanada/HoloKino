using UnityEngine;
using RenderHeads.Media.AVProVideo;

public class VideoSegments : MonoBehaviour
{
	public MediaPlayer _mediaPlayer;

	[Header("Videostart from in Ms:")]
	[Tooltip("Fill in where the video needs to start in milliseconds")]
	public int StartFrom;

	[Header("Videoloop start in Ms:")]
	[Tooltip("Fill in where the videoloop needs to start in milliseconds. If the video is al ready in a loop then it will go at the end of the loop to the beginning of the loop")]
	public int StartLoopFrom;

	[Header("Videoloop End in Ms: ")]
	[Tooltip("Fill in where the videoloop needs to end in milliseconds. If the video is past this point it will first go to the start of the video loop")]
	public int EndAt;

	void Start()
	{
		_mediaPlayer.m_Loop = true;
	}

	void Update()
	{
		if (_mediaPlayer.Control != null)
		{
			float currentTimeMs = _mediaPlayer.Control.GetCurrentTimeMs();
			if (currentTimeMs < StartFrom)
			{
				_mediaPlayer.Control.Seek(StartFrom);
			}
			else if (currentTimeMs > EndAt)
			{
				_mediaPlayer.Control.Seek(StartLoopFrom);
			}
		}
	}
}
