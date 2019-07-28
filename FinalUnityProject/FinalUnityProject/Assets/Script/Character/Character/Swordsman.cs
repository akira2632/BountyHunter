using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swordsman : ICharacterAI
{
    public Swordsman(ICharacter Character) :base(Character)
    {
        ChangeAIState(new IdleAIState());
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
