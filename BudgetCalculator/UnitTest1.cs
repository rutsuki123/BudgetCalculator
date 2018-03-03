using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BudgetCalculator
{
    [TestClass]
    public class UnitTest1
    {
        private IRepository<Budget> _repository = Substitute.For<IRepository<Budget>>();
        private BudgetCalculating target;

        [TestInitialize]
        public void TestInit()
        {
            target = new BudgetCalculating(_repository);
        }

        [TestMethod]
        public void 找不到這個月的預算_拿到0()
        {
            var target = BudgetCalculat(new List<Budget>() { new Budget() { YearMonth = "201801", Amount = 62 } });
            var start = new DateTime(2018, 2, 1);
            var end = new DateTime(2018, 2, 15);

            var actual = target.GetTotalAmount(start, end);

            actual.Should().Be(0);
        }

        [TestMethod]
        public void 時間起訖不合法()
        {
            GiveBudgets();

            Action actual = () => target.GetTotalAmount(new DateTime(2018, 3, 1), new DateTime(2018, 2, 1));

            actual.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void 當一月預算為62_一月一號到一月十五號_預算拿到30()
        {
            GiveBudgets(new Budget() { YearMonth = "201801", Amount = 62 });
            AmountShouldBe(30, new DateTime(2018, 1, 1), new DateTime(2018, 1, 15));
        }

        [TestMethod]
        public void 當一月預算為62_一月一號到一月三十一號_預算拿到62()
        {
            GiveBudgets(new Budget() { YearMonth = "201801", Amount = 62 });
            AmountShouldBe(62, new DateTime(2018, 1, 1), new DateTime(2018, 1, 31));
        }

        [TestMethod]
        public void 當一月預算為62_二月預算為0_三月預算為62_一月一號到三月十號_預算拿到82()
        {
            GiveBudgets(
                new Budget() { YearMonth = "201801", Amount = 62 },
                new Budget() { YearMonth = "201802", Amount = 0 },
                new Budget() { YearMonth = "201803", Amount = 62 }
            );

            AmountShouldBe(82, new DateTime(2018, 1, 1), new DateTime(2018, 3, 10));
        }

        [TestMethod]
        public void 當一月預算為62_二月預算為280_一月一號到二月二十八號_預算拿到342()
        {
            GiveBudgets(
                new Budget() { YearMonth = "201801", Amount = 62 },
                new Budget() { YearMonth = "201802", Amount = 280 }
            );

            AmountShouldBe(342, new DateTime(2018, 1, 1), new DateTime(2018, 2, 28));
        }

        [TestMethod]
        public void 當一月預算為62_二月預算為280_三月預算為62_一月一號到三月十號_預算拿到362()
        {
            GiveBudgets(
                new Budget() { YearMonth = "201801", Amount = 62 },
                new Budget() { YearMonth = "201802", Amount = 280 },
                new Budget() { YearMonth = "201803", Amount = 62 }
            );

            AmountShouldBe(362, new DateTime(2018, 1, 1), new DateTime(2018, 3, 10));
        }

        [TestMethod]
        public void 當十二月預算為310一月預算為310_二月預算為280_三月預算為310_十二月一號到三月十號_預算拿到1000()
        {
            GiveBudgets(
                new Budget() { YearMonth = "201712", Amount = 310 },
                new Budget() { YearMonth = "201801", Amount = 310 },
                new Budget() { YearMonth = "201802", Amount = 280 },
                new Budget() { YearMonth = "201803", Amount = 310 }
            );

            AmountShouldBe(1000, new DateTime(2017, 12, 1),new DateTime(2018, 3, 10));
        }

        [TestMethod]
        public void 資料庫沒預算()
        {
            GiveBudgets();

            AmountShouldBe(0, new DateTime(2018, 3, 1), new DateTime(2018, 3, 1));
        }

        private void AmountShouldBe(int expected, DateTime start, DateTime end)
        {
            target.GetTotalAmount(start, end).Should().Be(expected);
        }

        private BudgetCalculating BudgetCalculat(List<Budget> budgets)
        {
            IRepository<Budget> repo = Substitute.For<IRepository<Budget>>();
            repo.GetAll().Returns(budgets);

            return new BudgetCalculating(repo);
        }

        private void GiveBudgets(params Budget[] budgets)
        {
            _repository.GetAll().Returns(budgets.ToList());
        }
    }
}