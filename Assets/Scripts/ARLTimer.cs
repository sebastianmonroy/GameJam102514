using UnityEngine;
using System.Collections;

[System.Serializable]
public class ARLTimer {
	float StartTime;
	public float TimerInterval;
	public bool AutoRestart;
	
	public ARLTimer(float TimerInterval, bool AutoRestart){
		this.TimerInterval = TimerInterval; //time between timer going off (in seconds)
		this.AutoRestart = AutoRestart; //should the timer restart when it's over?
		this.Restart();
	}
	
	public ARLTimer(float TimerInterval) : this(TimerInterval, true){}
	
	public bool IsDone(){ //is the timer finished?
		if (Time.time > StartTime + TimerInterval){
			if (AutoRestart){
				Restart(); //restart timer when it's found to be over
			}
			return true;
		}
		else{
			return false;
		}
	}
			
	public void Restart(){
		StartTime = Time.time;
	}
	
	public void Restart(float percent){ //lets you set the timer already partially done (for some reason??)
		StartTime = Time.time - (TimerInterval*percent);
	}
	
	public float PercentDone(){
		return Mathf.Clamp((Time.time - StartTime) / TimerInterval, 0, 1);
	}
	
	public void SetDone(){
		StartTime = Time.time - TimerInterval;
	}
}
