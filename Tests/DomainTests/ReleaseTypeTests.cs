using Domain.MusicReleases;
using FluentAssertions;

namespace DomainTests;

public class ReleaseTypeTests
{
    [Fact]
    public void ReleaseTypeList_ShouldNotContainSimilarTypes()
    {
        // Arrange
        var typeList = ReleaseType.List;
        var nameSet = new HashSet<string>();
        // Act
        foreach (var type in typeList)
        {
            nameSet.Add(type.Name.ToLower());
        }
        // Assert
        typeList.Should().Match(tl => 
            tl.Count() == nameSet.Count);
    }
    
    [Fact]
    public void ReleaseTypeList_ShouldBeEnumerable()
    {
        // Arrange
        var typeList = ReleaseType.List;
        int sum = (typeList.Count+1)*typeList.Count / 2;
        int curSum = 0;
        // Act
        foreach (var type in typeList)
        {
            curSum+=type.Value;
        }
        // Assert
        sum.Should().Be(curSum);
    }
}