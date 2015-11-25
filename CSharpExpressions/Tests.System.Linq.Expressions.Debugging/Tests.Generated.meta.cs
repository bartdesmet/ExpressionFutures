using System.Linq.Expressions;

namespace Tests
{
    public partial class Tests
	{
        private Expression expr0 = Expression.Constant(1);
        private Expression expr1 = Expression.Add(Expression.Constant(1), Expression.Constant(2));

	    public void TestAll()
		{
            AssertDebug(expr0, dbg0);
            AssertDebug(expr1, dbg1);
        }
    }
}