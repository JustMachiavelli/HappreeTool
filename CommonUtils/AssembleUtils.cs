using System.Linq.Expressions;
using System.Reflection;

namespace HappreeTool.CommonUtils
{
    /// <summary>
    /// 获取类的属性，得到“按IEnumerable<string> parts拼接为字符串”的方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class AssembleUtils<T>
    {
        public static Func<T, string> CompileConcatHandler(IEnumerable<string> parts)
        {
            // parameter等于"T entity"，准备用于表达式树的传参
            var parameter = Expression.Parameter(typeof(T), "entity");
            // T的所有属性
            var properties = typeof(T).GetProperties();

            // 用于String.Concat()的string[]类型的参数，但是它不是一个具体的值，而是一个定义string[]的表达式
            var expressions = new List<Expression>();
            foreach (var part in parts)
            {
                // 属性部分获取对应属性的表达式
                var property = properties.FirstOrDefault(p => p.Name == part);
                if (property != null)
                {
                    // 取属性值的表达式，entity.Name
                    var propertyAccess = Expression.Property(parameter, property);
                    // 如果属性不是string，则需要进行类型转换
                    expressions.Add(
                        property.PropertyType == typeof(string) ?
                        propertyAccess : Expression.Convert(propertyAccess, typeof(object))
                    );
                }
                else
                {
                    // 如果找不到属性，则使用part的字符串值作为常量表达式
                    expressions.Add(Expression.Constant(part));
                }
            }

            // 将所有表达式转为string[]数组
            var stringArrayExpression = Expression.NewArrayInit(typeof(string), expressions);

            // 使用string.Concat(string[])方法
            MethodInfo methodInfo = typeof(string).GetMethod("Concat", new[] { typeof(string[]) })!;
            var joinExpression = Expression.Call(methodInfo, stringArrayExpression);
            var lambda = Expression.Lambda<Func<T, string>>(joinExpression, parameter);
            return lambda.Compile();
        }

        public static Func<T, IEnumerable<string>> CompilePropertiesHandler(IEnumerable<string> parts)
        {
            // 定义一个参数表达式，表示方法参数 "entity" 类型为 T
            var parameter = Expression.Parameter(typeof(T), "entity");

            // 获取 T 类型的所有属性信息
            var properties = typeof(T).GetProperties();

            // 用于存储生成的表达式列表
            var expressions = new List<Expression>();

            // 遍历所有需要处理的属性名
            foreach (var part in parts)
            {
                // 查找与当前属性名匹配的属性信息
                var property = properties.FirstOrDefault(p => p.Name == part);
                Expression expression;

                if (property != null)
                {
                    // 生成访问属性的表达式 "entity.PropertyName"
                    var propertyAccess = Expression.Property(parameter, property);

                    // 如果属性类型是字符串，则直接使用属性访问表达式
                    // 如果属性类型不是字符串，则转换为 object 类型
                    expression = property.PropertyType == typeof(string)
                        ? propertyAccess
                        : Expression.Convert(propertyAccess, typeof(object));
                }
                else
                {
                    // 如果找不到属性，则使用 part 的字符串值作为常量表达式
                    expression = Expression.Constant(part);
                }

                // 将生成的表达式添加到表达式列表中
                expressions.Add(expression);
            }

            // 创建一个数组表达式并用表达式列表中的元素填充
            var newArrayExpression = Expression.NewArrayInit(
                typeof(string),
                expressions.Select(e => Expression.Convert(e, typeof(string)))
            );

            // 生成 Lambda 表达式并编译为 Func<T, IEnumerable<string>> 类型的方法
            var lambda = Expression.Lambda<Func<T, IEnumerable<string>>>(newArrayExpression, parameter);

            return lambda.Compile();
        }

        public static Func<T, string> CompileTagHandler(IEnumerable<string> parts)
        {
            // 定义一个参数表达式，表示方法参数 "entity" 类型为 T
            var parameter = Expression.Parameter(typeof(T), "entity");

            // 获取 T 类型的所有属性信息
            var properties = typeof(T).GetProperties();

            // 用于存储生成的表达式列表
            var expressions = new List<Expression>();

            // 遍历所有需要生成标签的属性名
            foreach (var part in parts)
            {
                // 查找与当前属性名匹配的属性信息
                var property = properties.FirstOrDefault(p => p.Name == part);
                if (property != null)
                {
                    // 生成访问属性的表达式 "entity.PropertyName"
                    var propertyAccess = Expression.Property(parameter, property);

                    // 如果属性类型是字符串，则直接使用属性访问表达式
                    // 如果属性类型不是字符串，则转换为 object 类型
                    var valueAccess = property.PropertyType == typeof(string) ?
                        (Expression)propertyAccess : Expression.Convert(propertyAccess, typeof(object));

                    // 调用 ToString 方法以确保非字符串属性被转换为字符串
                    var toStringCall = Expression.Call(valueAccess, "ToString", Type.EmptyTypes);

                    // 生成表示字符串拼接的表达式 "string.Concat(string, string, string)"
                    var tagString = Expression.Call(
                        typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string), typeof(string) })!,
                        Expression.Constant($"  <tag>"),
                        toStringCall,
                        Expression.Constant($"</tag>\n")
                    );

                    // 将生成的标签字符串表达式添加到表达式列表中
                    expressions.Add(tagString);
                }
            }

            // 生成将所有标签字符串拼接在一起的表达式 "string.Concat(string[])"
            var joinExpression = Expression.Call(
                typeof(string).GetMethod("Concat", new[] { typeof(string[]) })!,
                Expression.NewArrayInit(typeof(string), expressions)
            );

            // 生成 Lambda 表达式并编译为 Func<T, string> 类型的方法
            var lambda = Expression.Lambda<Func<T, string>>(joinExpression, parameter);
            return lambda.Compile();
        }

    }
}
