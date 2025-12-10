using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[TestFixture]
public class BouncingBallTests
{
    private GameObject ball;
    private GameObject ground;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Load the BouncingBall scene before each test
        yield return SceneManager.LoadSceneAsync("BouncingBall", LoadSceneMode.Single);
    }

    [TearDown]
    public void TearDown()
    {
        if (ball != null)
        {
            Object.Destroy(ball);
        }
        if (ground != null)
        {
            Object.Destroy(ground);
        }
    }

    [UnityTest]
    public IEnumerator WhenSceneLoaded_ShouldHaveBallAtCorrectHeight()
    {
        // Arrange
        var expectedHeight = 5f;

        // Act
        ball = GameObject.Find("Ball");
        yield return null;

        // Assert
        Assert.IsNotNull(ball, "Ball GameObject should exist in scene");
        Assert.AreEqual(expectedHeight, ball.transform.position.y, 0.1f,
            "Ball should be positioned at 5 units height");
    }

    [UnityTest]
    public IEnumerator WhenSceneLoaded_BallShouldHaveRigidbodyWithGravity()
    {
        // Act
        ball = GameObject.Find("Ball");
        yield return null;

        // Assert
        var rigidbody = ball.GetComponent<Rigidbody>();
        Assert.IsNotNull(rigidbody, "Ball should have a Rigidbody component");
        Assert.IsTrue(rigidbody.useGravity, "Ball's Rigidbody should have gravity enabled");
    }

    [UnityTest]
    public IEnumerator WhenSceneLoaded_BallShouldHaveCollider()
    {
        // Act
        ball = GameObject.Find("Ball");
        yield return null;

        // Assert
        var collider = ball.GetComponent<Collider>();
        Assert.IsNotNull(collider, "Ball should have a Collider component");
    }

    [UnityTest]
    public IEnumerator WhenSceneLoaded_BallShouldHavePhysicsMaterialWithBounce()
    {
        // Act
        ball = GameObject.Find("Ball");
        yield return null;

        // Assert
        var collider = ball.GetComponent<Collider>();
        Assert.IsNotNull(collider.material, "Ball collider should have a physics material");
        Assert.Greater(collider.material.bounciness, 0f,
            "Physics material should have bounciness greater than 0");
    }

    [UnityTest]
    public IEnumerator WhenSceneLoaded_ShouldHaveGroundPlane()
    {
        // Act
        ground = GameObject.Find("Ground");
        yield return null;

        // Assert
        Assert.IsNotNull(ground, "Ground GameObject should exist in scene");
    }

    [UnityTest]
    public IEnumerator WhenSceneLoaded_GroundShouldBeStationary()
    {
        // Act
        ground = GameObject.Find("Ground");
        yield return null;

        // Assert
        var rigidbody = ground.GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            Assert.IsTrue(rigidbody.isKinematic,
                "Ground Rigidbody should be kinematic (stationary)");
        }
        // Ground can also be stationary by not having a Rigidbody at all
    }

    [UnityTest]
    public IEnumerator WhenSceneLoaded_GroundShouldHaveCollider()
    {
        // Act
        ground = GameObject.Find("Ground");
        yield return null;

        // Assert
        var collider = ground.GetComponent<Collider>();
        Assert.IsNotNull(collider, "Ground should have a Collider component");
    }

    [UnityTest]
    public IEnumerator WhenBallDrops_ShouldFallDueToGravity()
    {
        // Arrange
        ball = GameObject.Find("Ball");
        yield return null;
        var initialHeight = ball.transform.position.y;

        // Act - Wait for physics to take effect
        yield return new WaitForSeconds(0.5f);

        // Assert
        Assert.Less(ball.transform.position.y, initialHeight,
            "Ball should fall below initial height due to gravity");
    }

    [UnityTest]
    public IEnumerator WhenBallHitsGround_ShouldCollide()
    {
        // Arrange
        ball = GameObject.Find("Ball");
        ground = GameObject.Find("Ground");
        yield return null;

        // Act - Wait for ball to fall and hit ground
        yield return new WaitForSeconds(1.5f);

        // Assert - Ball should be near ground level
        var groundY = ground.transform.position.y;
        var ballRadius = ball.GetComponent<Collider>().bounds.extents.y;
        Assert.LessOrEqual(ball.transform.position.y, groundY + ballRadius + 0.5f,
            "Ball should have collided with ground and be near ground level");
    }

    [UnityTest]
    public IEnumerator WhenBallHitsGround_ShouldBounce()
    {
        // Arrange
        ball = GameObject.Find("Ball");
        yield return null;

        // Act - Wait for ball to fall and hit ground
        yield return new WaitForSeconds(1.0f);
        var positionAfterFirstImpact = ball.transform.position.y;

        // Wait a bit more to see if it bounces up
        yield return new WaitForSeconds(0.3f);
        var positionAfterBounce = ball.transform.position.y;

        // Assert - Ball should bounce back up
        Assert.Greater(positionAfterBounce, positionAfterFirstImpact,
            "Ball should bounce up after hitting the ground");
    }

    [UnityTest]
    public IEnumerator WhenBallBouncesMultipleTimes_ShouldEventuallyComeToRest()
    {
        // Arrange
        ball = GameObject.Find("Ball");
        yield return null;

        // Act - Wait for several seconds for multiple bounces
        yield return new WaitForSeconds(5.0f);

        var positionBefore = ball.transform.position.y;
        yield return new WaitForSeconds(0.5f);
        var positionAfter = ball.transform.position.y;

        // Assert - Ball should be nearly stationary
        Assert.AreEqual(positionBefore, positionAfter, 0.01f,
            "Ball should come to rest after multiple bounces");
    }
}
