using System;
using System.Collections;

public interface ICharacterAttachment
{
	IEnumerator Begain();

	void Pause();

	void Reset();

	void Resume();

	IEnumerator Current { get; set; }

	bool Paused { get; set; }

	bool ShouldPauseInFlypack { get; }

	StopFlag Stop { get; set; }
}
