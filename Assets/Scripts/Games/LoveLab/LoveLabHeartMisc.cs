using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace HeavenStudio.Games.Scripts_LoveLab
{
    public class LoveLabHeartMisc : MonoBehaviour
    {
        [SerializeField] GameObject heart;
        [SerializeField] GameObject completeHeart;
        public async void destroyObj()
        {         
            await Task.Delay(100);       
            Destroy(heart);
        } 

        public void createHeart()
        {
            Debug.Log("heart");
            //LoveLabHearts ch = Instantiate(completeHeart, LoveLab.instance.getHeartHolder()).GetComponent<LoveLabHearts>();
        }
    }
}

