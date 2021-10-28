using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Interactions;

namespace EdgeDriverTest
{
	[TestClass]
	public class EdgeDriverTest
	{
		private const string edgeDriverDirectory = @"C:\edgedriver_win64\";
		private EdgeDriver browser;
		private string url = "http://127.0.0.1:5500/index.html#/";

		[TestInitialize]
		public void EdgeDriverInitialize()
		{
			browser = new EdgeDriver(edgeDriverDirectory);
		}

		[TestMethod]	
		public void AddNote()
		{
			browser.Url = url;

			// Set expected result
			var expectedResult = "Example Task for Testing";

			// Add Add 1 task to the list
			var addNote = browser.FindElementByCssSelector("#note");
			//var addNote = browser.FindElementByCssSelector(".note-edit");
			addNote.SendKeys(expectedResult);
			addNote.SendKeys(Keys.Enter);
			
			// Verify the expected result with actual result
			var result = browser.FindElementByName("savedNote");
			Assert.AreEqual(expectedResult, result.Text);
		}

		[TestMethod]
		public void ItemsLeftCount()
		{
			browser.Url = url;
			var task = "Example Task for Testing";

			// Add 1 task to the list
			var addNote = browser.FindElementByCssSelector("#note");
			addNote.SendKeys(task);
			addNote.SendKeys(Keys.Enter);

			// Set expected result and verify the task count
			var expectedResult = "1 item left";
			var result = browser.FindElementByCssSelector(".pendingTasks");
			Assert.AreEqual(expectedResult, result.Text);

			// Mark the added task as completed
			var markTaskAsCompleted = browser.FindElementByCssSelector("[name='0']");
			markTaskAsCompleted.Click();

			// Update the expected result and verify the task count
			expectedResult = "0 items left";
			result = browser.FindElementByCssSelector(".pendingTasks");
			Assert.AreEqual(expectedResult, result.Text);

		}

		[TestMethod]
		public void AddThreeTasksAndCheckItemsLeftCount()
		{
			browser.Url = url;
			var task = "Example Task for Testing";

			// Add 3 tasks to the list
			var addNote = browser.FindElementByCssSelector("#note");
			
			addNote.SendKeys(task);
			addNote.SendKeys(Keys.Enter);

			addNote.SendKeys(task);
			addNote.SendKeys(Keys.Enter);

			addNote.SendKeys(task);
			addNote.SendKeys(Keys.Enter);

			// Set expected result and verify the task count
			var expectedResult = "3 items left";
			var result = browser.FindElementByCssSelector(".pendingTasks");
			Assert.AreEqual(expectedResult, result.Text);

			// Mark the first task as completed
			var markTaskAsCompleted = browser.FindElementByCssSelector("[name='0']");
			markTaskAsCompleted.Click();

			// Update the expected result and verify the task count
			expectedResult = "2 items left";
			result = browser.FindElementByCssSelector(".pendingTasks");
			Assert.AreEqual(expectedResult, result.Text);
		}


		[TestMethod]
		public void DoubleClickToEditTask()
		{

			browser.Url = url;

			var taskToEdit = "ABC";
			var extendedTask = "DEF";

			// Add Add 1 task to the list
			var addNote = browser.FindElementByCssSelector("#note");
			addNote.SendKeys(taskToEdit);
			addNote.SendKeys(Keys.Enter);

			// Find task label an double click
			var editTask = browser.FindElementByName("savedNote");
			var action = new Actions(browser);
			action.MoveToElement(editTask).DoubleClick().Perform();
			
			// Find input for editing
			var edit = browser.FindElementById("editNote");

			// Add the extendedTask to the end of taskToEdit 
			edit.SendKeys(extendedTask);
			edit.SendKeys(Keys.Enter);

			// Set expected result and verify the actual result
			var expectedResult = taskToEdit + extendedTask;
			var result = browser.FindElementByName("savedNote");
			Assert.AreEqual(expectedResult, result.Text);
		}


		[TestMethod]
		public void UrlManipulation()
		{
			browser.Url = url;
			
			/* Adding a new task to change the footer default value "display: none" to
			"display: block" since the footer has to be visible for this test.*/
			var newTask = "Example Task for Testing";
			var addNote = browser.FindElementByCssSelector("#note");
			addNote.SendKeys(newTask);
			addNote.SendKeys(Keys.Enter);

			// Test strings that should pass this Url Manipulation test.
			var viewActiveUrl = url + "active";
			var viewCompletedUrl = url + "completed";
			var viewAllUrl = url;

			// Switches to Active view and verify the string manipulation 
			var viewActive = browser.FindElementByCssSelector("#view-active");
			viewActive.Click();
			Assert.AreEqual(viewActiveUrl, browser.Url);

			// Switches to Completed view and verify the string manipulation 
			var viewCompleted = browser.FindElementByCssSelector("#view-completed");
			viewCompleted.Click();
			Assert.AreEqual(viewCompletedUrl, browser.Url);

			// Switches to All view and verify the string manipulation 
			var viewAll = browser.FindElementByCssSelector("#view-all");
			viewAll.Click();
			Assert.AreEqual(viewAllUrl, browser.Url);
		}

		[TestMethod]
		public void UrlManipulationFalsePositive()
		{
			browser.Url = url;

			/* Adding a new task to change the footer default value "display: none" to
			"display: block" since the footer has to be visible for this test.*/
			var newTask = "Example Task for Testing";
			var addNote = browser.FindElementByCssSelector("#note");
			addNote.SendKeys(newTask);
			addNote.SendKeys(Keys.Enter);

			// Test strings that should pass this Url Manipulation test
			var viewActiveUrl = url + "FAIL";
			var viewCompletedUrl = url + "FAIL";
			var viewAllUrl = url + "FAIL";

			// Switches to Active view and verify the string manipulation 
			var viewActive = browser.FindElementByCssSelector("#view-active");
			viewActive.Click();
			Assert.AreNotEqual(viewActiveUrl, browser.Url);

			// Switches to Completed view and verify the string manipulation 
			var viewCompleted = browser.FindElementByCssSelector("#view-completed");
			viewCompleted.Click();
			Assert.AreNotEqual(viewCompletedUrl, browser.Url);

			// Switches to All view and verify the string manipulation 
			var viewAll = browser.FindElementByCssSelector("#view-all");
			viewAll.Click();
			Assert.AreNotEqual(viewAllUrl, browser.Url);

		}

		[TestMethod]
		public void UrlManipulationWithBothEqualAndNotEqualAssertions()
		{
			browser.Url = url;

			/* Adding a new task to change the footer default value "display: none" to
			"display: block" since the footer has to be visible for this test.*/
			var newTask = "Example Task for Testing";
			var addNote = browser.FindElementByCssSelector("#note");
			addNote.SendKeys(newTask);
			addNote.SendKeys(Keys.Enter);

			// Test strings that should pass this Url Manipulation test.
			var viewActiveUrl = url + "FAIL";
			var viewCompletedUrl = url + "completed";
			var viewAllUrl = url;

			// Switches to Active view and verify the string manipulation 
			var viewActive = browser.FindElementByCssSelector("#view-active");
			viewActive.Click();
			Assert.AreNotEqual(viewActiveUrl, browser.Url);

			// Switches to Completed view and verify the string manipulation 
			var viewCompleted = browser.FindElementByCssSelector("#view-completed");
			viewCompleted.Click();
			Assert.AreEqual(viewCompletedUrl, browser.Url);

			// Switches to All view and verify the string manipulation 
			var viewAll = browser.FindElementByCssSelector("#view-all");
			viewAll.Click();
			Assert.AreEqual(viewAllUrl, browser.Url);
		}

		[TestMethod]
		public void MarkAllUncompletedTasksAsCompleted()
		{
			browser.Url = url;
			var task = "Example Task for Testing";

			// Get the input-element for adding/editing tasks
			var addNote = browser.FindElementByCssSelector("#note");

			// Add 3 new tasks to the list
			addNote.SendKeys(task);
			addNote.SendKeys(Keys.Enter);
			
			addNote.SendKeys(task);
			addNote.SendKeys(Keys.Enter);
			
			addNote.SendKeys(task);
			addNote.SendKeys(Keys.Enter);

			// Set expected result and verify the task count
			var expectedResult = "3 items left";
			var result = browser.FindElementByCssSelector(".pendingTasks");
			Assert.AreEqual(expectedResult, result.Text);

			//Get the element containing the toggle-button and click it
			var markAllTasksAsCompleted = browser.FindElementByCssSelector("#toggle-all");
			markAllTasksAsCompleted.Click();

			//Update expected result and verify the task count
			expectedResult = "0 items left";
			result = browser.FindElementByCssSelector(".pendingTasks");
			Assert.AreEqual(expectedResult, result.Text);
		}

		[TestMethod]
		// aka norrlands-testet
		public void SingularOrPluralForPendingTasks()
		{
			/*
			0  items left
			1 item left
			2 items left
			*/

			browser.Url = url;

			/* Adding a new task to change the footer default value "display: none" to
			"display: block" since the footer has to be visible for this test.*/
			var task = "Example Task for Testing";
			var addNote = browser.FindElementByCssSelector("#note");
			addNote.SendKeys(task);
			addNote.SendKeys(Keys.Enter);

			// Mark the added task as completed
			var markTaskAsCompleted = browser.FindElementByCssSelector("[name='0']");
			markTaskAsCompleted.Click();

			// Verify the task count
			var expectedResult = "0 items left";
			var result = browser.FindElementByCssSelector(".pendingTasks");
			Assert.AreEqual(expectedResult, result.Text);

			// Add new task and press enter
			addNote.SendKeys(task);
			addNote.SendKeys(Keys.Enter);

			// Update the expected result and verify the task count
			expectedResult = "1 item left";
			result = browser.FindElementByCssSelector(".pendingTasks");
			Assert.AreEqual(expectedResult, result.Text);

			// Add new task and press enter
			addNote.SendKeys(task);
			addNote.SendKeys(Keys.Enter);

			// Update the expected result and verify the task count
			expectedResult = "2 items left";
			result = browser.FindElementByCssSelector(".pendingTasks");
			Assert.AreEqual(expectedResult, result.Text);
		}



		[TestCleanup]
		public void EdgeDriverCleanup()
		{
			browser.Quit();
		}
	}
}
