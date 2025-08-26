using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SolutionDIcotaner
{
    // ������������ ��� ����������� ���� ����� ��������: Singleton (���� ���������)
    // ��� Transient (����� ��������� ��� ������ �������)
    public enum Lifetime
    {
        Singleton,
        Transient
    }

    // ����� DIContaner ��������� ��������� ������������ ��� ��������� ������������
    public class DIContaner : MonoBehaviour
    {
        // ������� ��� �������� ����������� ���������� (���� ��������� �� ���)
        private readonly Dictionary<Type, object> _singletons = new();
        // ������� ��� �������� ������, ��������� ����� ���������� ��� transient-��������
        private readonly Dictionary<Type, Func<object>> _factories = new();
        // ��������� ��� ������������ ��������, � ������� ��� �������� �����������, ����� �������� ����������� �������
        private readonly HashSet<object> _injectedObjects = new();

        // ����������� ������������ ��������� ������� � ��� ���� � ���������� ��� ��������
        public void Bind<T>(T insnance)
        {
            // ��������� ��������� � ������� ���������� �� ��� ����
            _singletons[insnance.GetType()] = insnance;
        }

        // ����������� ��� T � ����������, �������� ��������� �������������
        // �������� type ����������, ����� �� ��� �������� ��� transient
        public void Bind<T>(Lifetime type = Lifetime.Singleton) where T : class , new()
        {
            if(type == Lifetime.Singleton)
            {
                // ��� ��������� ������� ���� ��������� � ��������� ���
                _singletons[typeof(T)] = Activator.CreateInstance(typeof(T)); 
            }
            else
            {
                // ��� transient ������� �������, ������� ����� ��������� ����� ��������� ��� ������ �������
                _factories[typeof(T)] = () =>
                {
                    var insnance = Activator.CreateInstance(typeof(T));

                    // �������� ����������� � ����� ���������
                    Inject(insnance);

                    return insnance;
                };
            }
        }

        // ����������� ��������� TInterface � ���������� ���������� TImplementation
        // ��������� �������� ����������� ����� ���������
        public void Bind<TInterface, TImplementation>(Lifetime type = Lifetime.Singleton) where TImplementation : TInterface, new()
        {
            if (type == Lifetime.Singleton)
            {
                // ��� ��������� ������� ��������� ���������� � ��������� ��� ��� ������ ����������
                _singletons[typeof(TInterface)] = Activator.CreateInstance(typeof(TImplementation));
            }
            else
            {
                _factories[typeof(TInterface)] = () =>
                {
                    // ��� transient ������� �������, ������� ������� ����� ��������� ����������
                    var insnance = Activator.CreateInstance(typeof(TImplementation));

                    // �������� ����������� � ����� ���������
                    Inject(insnance);

                    return insnance;
                };
            }
        }

        // �������� ����������� � ��������� ������ (���� � ������ � ��������� [Inject])
        public void Inject(object instance)
        {
            // �������� ��� �������
            var type = instance.GetType();
            // ������� ��� ����, ���������� ��������� [Inject], ������� ��������� � ���������
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(field => field.IsDefined(typeof(InjectAttribute), true));

            // ������������ ������ ����
            foreach (var field in fields)
            {
                // ���������, ���� �� �������� ��� ���� ����
                if (_singletons.TryGetValue(field.FieldType, out var singleton))
                {
                    // ������������� �������� ���� �� ���������
                    field.SetValue(instance, singleton);

                    // ���� �������� ��� �� ���������, �������� � ���� �����������
                    if (!_injectedObjects.Contains(singleton))
                    {
                        Inject(singleton);

                        _injectedObjects.Add(singleton);
                    }
                }
                // ���� ��������� ���, ���������, ���� �� ������� ��� transient-�������
                else if (_factories.TryGetValue(field.FieldType, out var factory))
                {
                    field.SetValue(instance, factory?.Invoke());
                }
                else
                {
                    // ����� ��������������, ���� ����������� �� �������
                    Debug.Log($"{field.FieldType.Name} in {type.Name}");
                }
            }

            // ������� ��� ������, ���������� ��������� [Inject]
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(method => method.IsDefined(typeof(InjectAttribute), true));

            // ������������ ������ �����
            foreach (var method in methods)
            {
                // �������� ��������� ������
                var parameters = method.GetParameters();
                // ��������� ����������� ��� ���� ����������
                var args = ResolveAll(parameters);

                // ���� ��� ����������� ���������, �������� �����
                if (args != null)
                {
                    method.Invoke(instance, args);
                }
            }
        }

        // ��������� ����������� ��� ���� ���������� ������
        public object[] ResolveAll(ParameterInfo[] parameters)
        {
            // ������� ������ ��� �������� ����������
            var args = new object[parameters.Length];

            // ������������ ������ ��������
            for (int i = 0; i < parameters.Length; i++)
            {
                // ���������, ���� �� �������� ��� ���� ���������
                if (_singletons.TryGetValue(parameters[i].ParameterType, out var singleton))
                {
                    args[i] = singleton;

                    // ���� �������� ��� �� ���������, �������� � ���� �����������
                    if (!_injectedObjects.Contains(singleton))
                    {
                        Inject(singleton);

                        _injectedObjects.Add(singleton);
                    }
                }
                // ���� ��������� ���, ���������, ���� �� ������� ��� transient-�������
                else if (_factories.TryGetValue(parameters[i].ParameterType, out var factory))
                {
                    args[i] = factory?.Invoke();
                }
            }

            // ���������� ������ ����������
            return args;
        }

        // �������� ����������� �� ��� ������������������ ���������
        public void InjectAll()
        {
            // �������� �� ���� ���������� � �������� �����������
            foreach (var instance in _singletons.Values)
            {
                Inject(instance);
            }
        }
    }
}