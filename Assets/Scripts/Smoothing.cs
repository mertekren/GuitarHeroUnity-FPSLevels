using UnityEngine;

public class Smoothing
{
	private double visualOffset=0;
	private double smoothBpm = 0;
	private double smoothTick = 0;

	private static int MAX_SAMPLE_SIZE=100;
	private float[] fpslevels_samples = new float[MAX_SAMPLE_SIZE];
	private int fpslevels_last_index= 0;
	private int[] fpslevels_fps_values = new int[] { 30, 60, 120, 160 };
	private double fpslevels_current_moving_avg;


	public Smoothing(double _visualOffset)
	{
		visualOffset = _visualOffset;
	}
	public double SmoothBPM(double bpm)
	{
		smoothBpm = (smoothBpm == 0) ? bpm : ((smoothBpm * 0.9d) + (bpm * 0.1d));
		return smoothBpm;
	}
	public double SmoothTick(double tick, uint resolution)
	{
		double beatsPerSecond = smoothBpm / 60d;
		double secondsPassed = Time.deltaTime;
		double beatsPassed = beatsPerSecond * secondsPassed;
		double ticksPassed = beatsPassed * resolution;
		if (!double.IsNaN(ticksPassed) && smoothBpm > 0) smoothTick += ticksPassed;

		smoothTick= (smoothTick * 0.9d) + (tick * 0.1d);

		double offsetSeconds = visualOffset * 0.001d;
		double offsetBeats = beatsPerSecond * offsetSeconds;
		double offsetTicks = offsetBeats * resolution;

		return smoothTick + offsetTicks;
	}
	public void SmoothFPS(float millisecondsPassed)
	{
		Debug.Log("SmoothFPS in action\n");

		if (fpslevels_last_index == MAX_SAMPLE_SIZE)
			fpslevels_last_index = 0;

		fpslevels_samples[fpslevels_last_index] = millisecondsPassed;
		fpslevels_last_index++;

		fpslevels_current_moving_avg = fpslevels_moving_avg(fpslevels_samples);
		int fpslevels_avg_fps = (int) (1000 / fpslevels_current_moving_avg);
		Debug.Log("avg fps = " + fpslevels_avg_fps + "\n");

		int targetFrameRate = 160;
		// level the fps
		for(int i=0; i<4; i++ )
        {
			if(fpslevels_avg_fps < fpslevels_fps_values[i])
            {
				if (i == 0)
					targetFrameRate = 10;
				else
					targetFrameRate = fpslevels_fps_values[i-1];
			}
        }
		Application.targetFrameRate = targetFrameRate;
	}

	private double fpslevels_moving_avg(float[] samples)
    {
		int i = 0;
		double avg=0;
		for(i=0; i<MAX_SAMPLE_SIZE; i++)
        {
			avg += ( (double) samples[i] ) / MAX_SAMPLE_SIZE;
        }

		return avg;
    }
}
