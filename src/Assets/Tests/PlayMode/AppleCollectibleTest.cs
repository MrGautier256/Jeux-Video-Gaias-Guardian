using NUnit.Framework;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;


public class AppleCollectibleTest
{

    [UnityTest]
    public IEnumerator TestSceneLoadsAndContainsApple()
    {
        // Charge la scene de test
        SceneManager.LoadScene("Level_1");

        // Attendre une frame que la scene charge
        yield return null;

        // Vérifie qu'un objet nomme "Apple" est present dans la scene
        var apples = GameObject.FindGameObjectsWithTag("Apple");
        Assert.IsTrue(apples.Count() > 0, "L'objet 'Apple' devrait etre present dans la scene.");
    }

}
