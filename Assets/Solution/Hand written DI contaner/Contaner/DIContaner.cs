using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SolutionDIcotaner
{
    // Перечисление для определения типа жизни объектов: Singleton (один экземпляр)
    // или Transient (новый экземпляр при каждом запросе)
    public enum Lifetime
    {
        Singleton,
        Transient
    }

    // Класс DIContaner реализует контейнер зависимостей для внедрения зависимостей
    public class DIContaner : MonoBehaviour
    {
        // Словарь для хранения экземпляров синглтонов (один экземпляр на тип)
        private readonly Dictionary<Type, object> _singletons = new();
        // Словарь для хранения фабрик, создающих новые экземпляры для transient-объектов
        private readonly Dictionary<Type, Func<object>> _factories = new();
        // Множество для отслеживания объектов, в которые уже внедрены зависимости, чтобы избежать циклических вызовов
        private readonly HashSet<object> _injectedObjects = new();

        // Привязывает существующий экземпляр объекта к его типу в контейнере как синглтон
        public void Bind<T>(T insnance)
        {
            // Сохраняем экземпляр в словаре синглтонов по его типу
            _singletons[insnance.GetType()] = insnance;
        }

        // Привязывает тип T к контейнеру, создавая экземпляр автоматически
        // Параметр type определяет, будет ли это синглтон или transient
        public void Bind<T>(Lifetime type = Lifetime.Singleton) where T : class , new()
        {
            if(type == Lifetime.Singleton)
            {
                // Для синглтона создаем один экземпляр и сохраняем его
                _singletons[typeof(T)] = Activator.CreateInstance(typeof(T)); 
            }
            else
            {
                // Для transient создаем фабрику, которая будет создавать новый экземпляр при каждом запросе
                _factories[typeof(T)] = () =>
                {
                    var insnance = Activator.CreateInstance(typeof(T));

                    // Внедряем зависимости в новый экземпляр
                    Inject(insnance);

                    return insnance;
                };
            }
        }

        // Привязывает интерфейс TInterface к конкретной реализации TImplementation
        // Позволяет внедрять зависимости через интерфейс
        public void Bind<TInterface, TImplementation>(Lifetime type = Lifetime.Singleton) where TImplementation : TInterface, new()
        {
            if (type == Lifetime.Singleton)
            {
                // Для синглтона создаем экземпляр реализации и сохраняем его под ключом интерфейса
                _singletons[typeof(TInterface)] = Activator.CreateInstance(typeof(TImplementation));
            }
            else
            {
                _factories[typeof(TInterface)] = () =>
                {
                    // Для transient создаем фабрику, которая создает новый экземпляр реализации
                    var insnance = Activator.CreateInstance(typeof(TImplementation));

                    // Внедряем зависимости в новый экземпляр
                    Inject(insnance);

                    return insnance;
                };
            }
        }

        // Внедряет зависимости в указанный объект (поля и методы с атрибутом [Inject])
        public void Inject(object instance)
        {
            // Получаем тип объекта
            var type = instance.GetType();
            // Находим все поля, помеченные атрибутом [Inject], включая публичные и приватные
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(field => field.IsDefined(typeof(InjectAttribute), true));

            // Обрабатываем каждое поле
            foreach (var field in fields)
            {
                // Проверяем, есть ли синглтон для типа поля
                if (_singletons.TryGetValue(field.FieldType, out var singleton))
                {
                    // Устанавливаем значение поля из синглтона
                    field.SetValue(instance, singleton);

                    // Если синглтон еще не обработан, внедряем в него зависимости
                    if (!_injectedObjects.Contains(singleton))
                    {
                        Inject(singleton);

                        _injectedObjects.Add(singleton);
                    }
                }
                // Если синглтона нет, проверяем, есть ли фабрика для transient-объекта
                else if (_factories.TryGetValue(field.FieldType, out var factory))
                {
                    field.SetValue(instance, factory?.Invoke());
                }
                else
                {
                    // Пишем предупреждение, если зависимость не найдена
                    Debug.Log($"{field.FieldType.Name} in {type.Name}");
                }
            }

            // Находим все методы, помеченные атрибутом [Inject]
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(method => method.IsDefined(typeof(InjectAttribute), true));

            // Обрабатываем каждый метод
            foreach (var method in methods)
            {
                // Получаем параметры метода
                var parameters = method.GetParameters();
                // Разрешаем зависимости для всех параметров
                var args = ResolveAll(parameters);

                // Если все зависимости разрешены, вызываем метод
                if (args != null)
                {
                    method.Invoke(instance, args);
                }
            }
        }

        // Разрешает зависимости для всех параметров метода
        public object[] ResolveAll(ParameterInfo[] parameters)
        {
            // Создаем массив для хранения аргументов
            var args = new object[parameters.Length];

            // Обрабатываем каждый параметр
            for (int i = 0; i < parameters.Length; i++)
            {
                // Проверяем, есть ли синглтон для типа параметра
                if (_singletons.TryGetValue(parameters[i].ParameterType, out var singleton))
                {
                    args[i] = singleton;

                    // Если синглтон еще не обработан, внедряем в него зависимости
                    if (!_injectedObjects.Contains(singleton))
                    {
                        Inject(singleton);

                        _injectedObjects.Add(singleton);
                    }
                }
                // Если синглтона нет, проверяем, есть ли фабрика для transient-объекта
                else if (_factories.TryGetValue(parameters[i].ParameterType, out var factory))
                {
                    args[i] = factory?.Invoke();
                }
            }

            // Возвращаем массив аргументов
            return args;
        }

        // Внедряет зависимости во все зарегистрированные синглтоны
        public void InjectAll()
        {
            // Проходим по всем синглтонам и внедряем зависимости
            foreach (var instance in _singletons.Values)
            {
                Inject(instance);
            }
        }
    }
}