/*using UnityEngine;

namespace GILES.Gameplay
{
  public class GILES_PlaymodeAPI : MonoBehaviour
  {
    List<GILES_MonoBehaviour> behaviours = new List<GILES_MonoBehaviour>();
    
    public enum WorldState {NotRunning, Playing, Pause}
    static WorldState state;
    static bool init;
      
    void Init(){
      for(int i = 0; i < behaviours.Count; i++){
        behaviours[i].OnStart();
      }
    }
    void Update(){
    if (state == WorldState.Playing && !init){
        AwakeInit();
    }
    if (init && state == WorldState.NotRunning)
      init = false;
      
      if (state == WorldState.NotRunning)
        return;
        
      for(int i = 0; i < behaviours.Count; i++){
        behaviours[i].OnUpdate();
      }   
    }
    void AwakeInit(){
      for(int i = 0; i < behaviours.Count; i++){
        behaviours[i].OnAwake();
      }
      Init();
    }
    void LateUpdate(){
      if (state == WorldState.NotRunning)
        return;
        
      for(int i = 0; i < behaviours.Count; i++){
       behaviours[i].OnLateUpdate();
      }
    }

    public void Play(){
      if (state == Playing){
      Debug.Log ("Already playing");
      }
      if (state == WorldState.Pause){
      Debug.Log ("You have paused");
      }
      if (state == WorldState.NotRunning){
        state = WorldState.Playing;
      }
    }
    public void Pause(){
      if (state == Playing){
        state = WorldState.Pause;
      }
      if (state == WorldState.Pause){
        state = WorldState.Playing;
      }
      if (state == WorldState.NotRunning){
        Debug.Log ("The game isn't running");
      }
    }
    public void Stop(){
    
    }
    public void ChangeScene(){
    
    }

    public void RegisterGILESMonoBehaviour(GILES_MonoBehaviour t){
        behaviours.Add(t);
    }
    public void DeRegisterGILESMonoBehaviour(GILES_MonoBehaviour t){
        behaviours.Remove(t);
    }
  }

}*/
