using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using ExtendedXmlSerializer;
using ExtendedXmlSerializer.Configuration;
using Xunit;

namespace InterfaceListSerialization
{
    public interface IMyItem
    {
        string Name { get; set; }
    }

    public class MyItemWithInt : IMyItem
    {
        public MyItemWithInt()
        {
        }

        public MyItemWithInt(string name, int integer)
        {
            Name = name;
            Integer = integer;
        }

        public int Integer { get; set; }

        public string Name { get; set; }
    }

    public class MyItemWithDouble : IMyItem
    {
        public MyItemWithDouble()
        {
        }

        public MyItemWithDouble(string name, double d)
        {
            Name = name;
            Double = d;
        }

        public double Double { get; set; }

        public string Name { get; set; }
    }

    public interface IMyClass
    {
    }

    public interface IMyClass<T> : IMyClass where T : IMyItem
    {
        List<T> MyItems { get; set; }
    }

    public class MyClassOfInts : IMyClass<MyItemWithInt>
    {
        public int OneOfThem { get; set; }
        public List<MyItemWithInt> MyItems { get; set; }
    }

    public class MyClassOfDoubles : IMyClass<MyItemWithDouble>
    {
        public double Sample { get; set; }
        public List<MyItemWithDouble> MyItems { get; set; }
    }

    public static class SampleDataGenerator
    {
        public static List<IMyClass> GenerateSample1()
        {
            var sample = new List<IMyClass>
            {
                new MyClassOfInts
                {
                    MyItems = new List<MyItemWithInt>
                    {
                        new MyItemWithInt("uno", 1),
                        new MyItemWithInt("dos", 2)
                    },
                    OneOfThem = 3
                },
                new MyClassOfInts
                {
                    MyItems = new List<MyItemWithInt>
                    {
                        new MyItemWithInt("cuatro", 4),
                        new MyItemWithInt("cinco", 5)
                    },
                    OneOfThem = 6
                },
                new MyClassOfDoubles
                {
                    MyItems = new List<MyItemWithDouble>
                    {
                        new MyItemWithDouble("1", 1),
                        new MyItemWithDouble("2", 2)
                    },
                    Sample = 1
                },
                new MyClassOfDoubles
                {
                    MyItems = new List<MyItemWithDouble>
                    {
                        new MyItemWithDouble("3", 3),
                        new MyItemWithDouble("4", 4)
                    },
                    Sample = 5
                }
            };
            return sample;
        }
    }

    public class UnitTest1
    {
        [Fact]
        public void TestSerializeDataAndRead()
        {
            var sample = SampleDataGenerator.GenerateSample1();
            Assert.NotNull(sample);
            Assert.NotEmpty(sample);
            var firstElement = sample.FirstOrDefault() as MyClassOfInts;
            Assert.NotNull(firstElement);
            Assert.Equal(3, firstElement.OneOfThem);
            var firstItem = firstElement.MyItems.FirstOrDefault();
            Assert.NotNull(firstItem);
            Assert.Equal(1, firstItem.Integer);
            Assert.Equal("uno", firstItem.Name);

            var thirdElement = sample.Skip(2).FirstOrDefault() as MyClassOfDoubles;
            Assert.NotNull(thirdElement);
            Assert.Equal(1, thirdElement.Sample);
            var thirdItem = thirdElement.MyItems.FirstOrDefault();
            Assert.NotNull(thirdItem);
            Assert.Equal(1, thirdItem.Double);
            Assert.Equal("1", thirdItem.Name);

            var serializer = new ConfigurationContainer()
                .UseAutoFormatting()
                .UseOptimizedNamespaces()
                .Create();

            var xml = serializer.Serialize(new XmlWriterSettings {Indent = true}, sample);
            File.WriteAllText("sample1.xml", xml);


            var readFile = File.ReadAllText("sample1.xml");
            var readSample = serializer.Deserialize<List<IMyClass>>(readFile);
            Assert.NotNull(readSample);
            Assert.NotEmpty(readSample);
            var firstElementRead = readSample.FirstOrDefault() as MyClassOfInts;
            Assert.NotNull(firstElementRead);
            Assert.Equal(3, firstElementRead.OneOfThem);
            var firstItemRead = firstElementRead.MyItems.FirstOrDefault();
            Assert.NotNull(firstItemRead);
            Assert.Equal(1, firstItemRead.Integer);
            Assert.Equal("uno", firstItemRead.Name);

            var thirdElementRead = readSample.Skip(2).FirstOrDefault() as MyClassOfDoubles;
            Assert.NotNull(thirdElementRead);
            Assert.Equal(1, thirdElementRead.Sample);
            var thirdItemRead = thirdElementRead.MyItems.FirstOrDefault();
            Assert.NotNull(thirdItemRead);
            Assert.Equal(1, thirdItemRead.Double);
            Assert.Equal("1", thirdItemRead.Name);
        }
    }
}