using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor.TestTools;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class PlayerTests
{
    private GameObject eventGeneratorPrefab;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        eventGeneratorPrefab = Resources.Load<GameObject>("Test Prefabs/EventGeneratorPrefab");
        SceneManager.CreateScene("Test Scene");
        GameObject eventGeneratorInstance = Object.Instantiate(eventGeneratorPrefab);
    }

    [UnityTest]
    public IEnumerator TestMaximumHealthLimit()
    {
        Character testCharacter = CharacterFactory.CreateCharacter(CharacterType.Carpenter);
        Player testPlayer = new Player(0, "Test Player", testCharacter);

        int healthIncrease = 10;
        testPlayer.ModifyHealth(healthIncrease);

        yield return new WaitForSeconds(0.25f);

        Assert.That(testPlayer.health, Is.LessThanOrEqualTo(testCharacter.maximumHealth));
        Debug.Log("MaximumHealthLimit Test Result: " + (testPlayer.health <= testCharacter.maximumHealth));
    }

    [UnityTest]
    public IEnumerator TestHealthCantBeNegative()
    {
        Character testCharacter = CharacterFactory.CreateCharacter(CharacterType.Carpenter);
        Player testPlayer = new Player(0, "Test Player", testCharacter);

        int healthIncrease = -50;
        testPlayer.ModifyHealth(healthIncrease);

        yield return new WaitForSeconds(0.25f);

        Assert.That(testPlayer.health, Is.GreaterThanOrEqualTo(0));
        Debug.Log("TestHealthCantBeNegative Test Result: " + (testPlayer.health >= 0));
    }
}
