using MediatR;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGeekFan.FreeKit.Extras.Domain;


public interface IDomainEvent : INotification
{
}

/// <summary>
/// 表示一个领域事件处理程序
/// </summary>
/// <typeparam name="TDomainEvent"></typeparam>
public interface IDomainEventHandler<in TDomainEvent> : INotificationHandler<TDomainEvent> where TDomainEvent : IDomainEvent
{

}

public interface IDomainEventBase
{
    List<IDomainEvent> DomainEvents { get; set; }
    public IReadOnlyList<IDomainEvent> GetDomainEvents() => DomainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent eventItem) => DomainEvents.Add(eventItem);

    public void RemoveDomainEvent(IDomainEvent eventItem) => DomainEvents.Remove(eventItem);

    public void ClearDomainEvents() => DomainEvents.Clear();
    object[] GetKeys();
    public virtual string ToString() => "[Entity: " + GetType().Name + "] Keys = " + string.Join(",", GetKeys());
}

public abstract class DomainEventBase : IDomainEventBase
{
    public List<IDomainEvent> DomainEvents { get; set; } = new();

    public abstract object[] GetKeys();

    public override string ToString() => "[Entity: " + GetType().Name + "] Keys = " + string.Join(",", GetKeys());
}
