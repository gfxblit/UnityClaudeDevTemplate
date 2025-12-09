using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SimplePlayModeTest
{
    [UnityTest]
    public IEnumerator CreateGameObject_ShouldSucceed()
    {
        // Arrange
        var gameObjectName = "TestObject";

        // Act
        var gameObject = new GameObject(gameObjectName);
        yield return null; // Wait one frame

        // Assert
        Assert.IsNotNull(gameObject);
        Assert.AreEqual(gameObjectName, gameObject.name);

        // Cleanup
        Object.Destroy(gameObject);
    }

    [UnityTest]
    public IEnumerator GameObject_WithTransform_ShouldHavePosition()
    {
        // Arrange
        var gameObject = new GameObject("TestObject");
        var expectedPosition = new Vector3(1f, 2f, 3f);

        // Act
        gameObject.transform.position = expectedPosition;
        yield return null; // Wait one frame

        // Assert
        Assert.AreEqual(expectedPosition, gameObject.transform.position);

        // Cleanup
        Object.Destroy(gameObject);
    }

    [Test]
    public void SimpleAssertion_ShouldPass()
    {
        // Arrange
        var expected = 42;

        // Act
        var actual = 42;

        // Assert
        Assert.AreEqual(expected, actual);
    }
}
