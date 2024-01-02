using System.Reflection;
using Domain.Abstractions;
using FluentAssertions;

namespace DomainTests;

public class EntityTests
{
    public class EntityStub
        : Entity<EntityStub>
    {
        public int Test { get; protected set; }

        public EntityStub()
        {
            
        }

        public EntityStub(int test): base(Guid.NewGuid())
        {
            Test = test;
        }
        public static EntityStub Create(int test)
        {
            return new(test);
        }
        
        public static EntityStub Create(int test, DomainEvent domainEvent)
        {
            var entity = new EntityStub(test);
            entity.RaiseEvent(domainEvent);
            return entity;
        }
    }

    public record EventStub()
        : DomainEvent(Guid.NewGuid());
    
    [Fact]
    public void Equals_ShouldReturnFalse_OnCompareWithDifferentType()
    {
        // Arrange
        var user = EntityStub.Create(1);
        
        // Act
        var res = user.Equals(new object());
        
        // Assert
        res.Should().BeFalse();
    }

    [Fact]
    public void Equals_ShouldReturnTrue_OnCompareWithSameId()
    {
        // Arrange
        var user = EntityStub.Create(1);
        
        // Act
        var res = user.Equals((object)user);
        
        // Assert
        res.Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_ShouldReturnSameValue_OnSameId()
    {
        // Arrange
        var entity1 = EntityStub.Create(1);
        var entity2 = EntityStub.Create(2);

        var idprop = entity2.GetType().GetProperty("Id", BindingFlags.Instance | BindingFlags.Public)!;
        
        idprop.SetValue(entity2, entity1.Id);
        
        // Act
        var res = entity1.GetHashCode() != entity2.GetHashCode();
        
        // Assert
        res.Should().BeFalse();
    }

    [Fact]
    public void RaiseEvent_ShouldThrowException_IfTheEventIsNull()
    {
        // Arrange
        var action = () => EntityStub.Create(1, null!);
        
        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void RaiseEvent_ShouldRaiseEvent_IfTheEventIsValid()
    {
        // Arrange
        var eventStub = new EventStub();
        var entity = EntityStub.Create(1, eventStub);
        
        // Assert
        entity.DomainEvents.Should().Contain(eventStub);
    }
}