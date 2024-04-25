using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BGsoundWidget :  FocusWidget{

    AudioSource source;
    AudioClip clip;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void SetSound(AudioClip clip){
        this.clip = clip;
    }

    override
    public void HandleonFadeInStart(float time){
        source.volume = 0;
        source.loop = true;
        // source.clip = clip;
        source.Play();
        source.DOFade(1,time);

    }
    
    override
    public void HandleonFadeOutStart(float time){
        source.DOFade(0,time);
        
    }

    public override void HandleonFadeComplete()
    {
        source.Stop();
    }

}

public class TodoList : MonoBehaviour{

    public List<ListItem> listItems;
}

public class ListItem : MonoBehaviour{

    // virtual
    // public 

}

public class RecordingListItem : ListItem{

}

public class TextListItem : ListItem{

}
