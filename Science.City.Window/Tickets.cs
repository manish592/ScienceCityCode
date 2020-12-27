using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Science.City.Window.Helpers;
using System.Collections;
using static Science.City.Window.Helpers.EnumHelper;
using static Science.City.Window.Helpers.Common;
using System.Data.SqlClient;
using System.Threading;
using System.Globalization;

namespace Science.City.Window
{
	public partial class Tickets : BaseForm
	{
		ComboBox[] cmb;
		Label[] lb;
		string appsetting = ConfigurationManager.AppSettings["Api"];
		string MyTax = ConfigurationManager.AppSettings["Tax"];
		string Dateformat = ConfigurationManager.AppSettings["MyDate"];
		public string gt;
		DataTable dt = new DataTable();
		DataRow dr;
		string Path;
		bool check;
		string jsonTicketPackage;
		string jsonTicketPackageCategory;
		string jsonTicketPackageTimming;
		string jsonTicketPrintdata;
		string PackageFK;
		static int add;
		static int add1;
		string itemFk;
		string TicketPackageId;
		public static string Tkt_Id;
		int addTk;
		int totalticketsCount;
		decimal FinalTotalCash = 0;
		public static string PaymentStatus;
		int j = 0;
		public static string pakname;
		public static string Checkthirdparty;
		public static int l = 1;
		ComboBox newBox = null;
		int countotal;
		ArrayList lsttimming = new ArrayList();
		ArrayList EnterTimming = new ArrayList();
		ArrayList ThirdPartyLogin = new ArrayList();
		Common ht = new Common();
		int Amount;
		int DiscountTk;
		int discountAmountTk;
		decimal NeTAmountTk;
		int maxcouintitem = 0;
		bool countvalues = false;
		int FinalPricePer;
		int countcolumns;
		string itemidTickets;
		int findlstvalues = 1;
		string FindTicketsid;
		string MislaneousCharges = "0";
		int createId;
		public static string Timepackage;
		public static string PackageTime;
		public static string ThirdpartyId;
		Random rnd = new Random();
		public static string Myvisitdate;
		string data;
		int Totalmembercalculate = 0;
		string UserFk;
		string CouponFk;
		public Tickets()
		{
			InitializeComponent();
			ReprintTicket.PrintForms = null;
			ParkingTicket.Parking = null;
			Inventory.CashMemo.Inventory = null;
			ParkingTicket.mislaneous = null;
			monthCalendar1.MaxSelectionCount = 1;
			lst_Package.Visible = true;
			lbl_package.Visible = true;
			lbl_package.Text = "Tickets";
			pakname = "Tickets";
			GetTickets();
			txt_bookingdate.Text = DateTime.Now.ToShortDateString();
			System.DateTime today = this.monthCalendar1.TodayDate;
			txt_visitdate.Text = today.ToString();
			UserFk = LoginControl.ID;
			CouponFk = null;
			if (LoginControl.RoleNames == GetDescription(Role.THIRDPARTY))
			{
				lbl_searchs.Visible = true;
				txt_search.Visible = true;
				but_search.Visible = true;
			}
			if (StartBooking.StaffFk != null || StartBooking.Schoolfk != null)
			{
				ShowUserDetails();
				lblstring1.Text = StartBooking.Noofmembers.ToString();
				lblstring.Visible = true;
				lblstring1.Visible = true;
				UserFk = StartBooking.UserFk;
				CouponFk = StartBooking.CouponFk;
				if (StartBooking.Schoolfk != null)
				{
					lblstring1.Text = lblstring1.Text + "(" + "Teacher= " + StartBooking.Noofteacher + ", " + "Students= " + StartBooking.NoofStudents + ", " + "Complementary= " + StartBooking.NoofCompliementary + ")";
				}
			}
		}

		#region METHOD

		//Package

		public void GetPAckage()
		{
			lst_Package.Items.Clear();
			string data = Common.GetData(string.Format("{0}/api/package/GetPackage?UserId={1}", appsetting, LoginControl.ID));
			var jsonObject = JsonConvert.DeserializeObject<List<Common.RootObject>>(data);
			if (jsonObject != null)
			{
				foreach (var Package in jsonObject)
				{
					Common.ComboboxItem item = new Common.ComboboxItem();
					item.Text = Package.packageName;
					item.Value = Package.id;
					lst_Package.Items.Add(item);
					lst_Package.SelectionMode = SelectionMode.One;
				}
			}
		}

		public void PackageGrid()
		{
			this.Invoke(new MethodInvoker(delegate
			{
				//Control.CheckForIllegalCrossThreadCalls = false;
				dgridCategory.ClearSelection();
				dgridCategory.Columns.Clear();
				int i1 = 1;
				dt.Columns.Clear();
				dt.Rows.Clear();
				dr = dt.NewRow();
				dt.Columns.Add("SrNo");
				dt.Columns.Add("Category");
				if (LoginControl.RoleNames == GetDescription(Role.THIRDPARTY))
				{
					data = Common.GetData(string.Format("{0}/api/Category?IsThirdParty={1}", appsetting, true));
				}
				else
				{
					data = Common.GetData(string.Format("{0}/api/Category", appsetting));
				}
				var jsonObject = JsonConvert.DeserializeObject<List<Common.Category>>(data);
				foreach (Common.ComboboxItem item in lst_Package.CheckedItems)
				{
					var propertyInfo = item.GetType().GetProperty("Text");
					var TextPackage = propertyInfo.GetValue(item, null);
					dt.Columns.Add(TextPackage.ToString());
					dt.Columns.Add("Discount");
					dt.Columns.Add("NetPrice");
					dt.Columns.Add("NoofMembers");
					dt.Columns.Add("Complimentary");
					dt.Columns.Add("Amount");
					dt.Columns.Add("groupAmount");
					dt.Columns.Add("groupDiscountAmount");
					dt.Columns.Add("totalMembers");
					dt.Columns.Add("itemid");
					dt.Columns.Add("CategoryidPackage");
					dt.Columns.Add("packageDiscountAmount");
					dt.Columns.Add("isGroup");
					dt.Columns.Add("Compli");
					dt.Columns.Add("Price");
					dt.Columns.Add("NoofAdults");
					dt.Columns.Add("NoofMinors");
					foreach (var categories in jsonObject)
					{
						lblpackname.Text = TextPackage.ToString();
						var PackageTime = ht.time(DateTime.Parse(DateTime.Now.ToString()));
						dr = dt.NewRow();
						dr["SrNo"] = i1;
						dr["Category"] = categories.categoryName;
						Path = string.Format("{0}/api/Package/GetPackagePrice?PackageId={1}&CategoryId={2}&Date={3}", appsetting, item.Value, categories.id, PackageTime);
						string Amount = Common.GetData(Path);
						var jsonObject1 = JsonConvert.DeserializeObject<Common.amounts>(Amount);
						dr[TextPackage.ToString()] = jsonObject1.amount;
						dr["Discount"] = Convert.ToInt32(jsonObject1.discount);
						dr["NetPrice"] = (jsonObject1.amount - jsonObject1.packageDiscountAmount);
						dr["NoofMembers"] = 0;
						dr["Complimentary"] = 0;
						dr["Amount"] = 0;
						dr["groupAmount"] = jsonObject1.groupAmount;
						dr["groupDiscountAmount"] = jsonObject1.groupDiscountAmount;
						dr["totalMembers"] = categories.totalMembers;
						dr["itemid"] = item.Value;
						dr["CategoryidPackage"] = categories.id;
						dr["packageDiscountAmount"] = jsonObject1.packageDiscountAmount;
						dr["isGroup"] = categories.isGroup;
						dr["Compli"] = categories.complimentary;
						dr["Price"] = jsonObject1.amount;
						dr["NoofAdults"] = categories.NoofAdults;
						dr["NoofMinors"] = categories.NoofMinors;
						i1++;
						dt.Rows.Add(dr);
					}
					dgridCategory.DataSource = dt;
					dgridCategory.Columns[0].ReadOnly = true;
					dgridCategory.Columns[1].ReadOnly = true;
					dgridCategory.Columns[2].ReadOnly = true;
					dgridCategory.Columns[3].ReadOnly = true;
					dgridCategory.Columns[4].ReadOnly = true;
					dgridCategory.Columns[6].ReadOnly = true;
					dgridCategory.Columns[7].ReadOnly = true;
					dgridCategory.Columns["groupAmount"].Visible = false;
					dgridCategory.Columns["groupDiscountAmount"].Visible = false;
					dgridCategory.Columns["totalMembers"].Visible = false;
					dgridCategory.Columns["itemid"].Visible = false;
					dgridCategory.Columns["CategoryidPackage"].Visible = false;
					dgridCategory.Columns["packageDiscountAmount"].Visible = false;
					dgridCategory.Columns["isGroup"].Visible = false;
					dgridCategory.Columns["Compli"].Visible = false;
					dgridCategory.Columns["Price"].Visible = false;
					dgridCategory.Columns["NoofAdults"].Visible = false;
					dgridCategory.Columns["NoofMinors"].Visible = false;
					dgridCategory.Visible = true;
					groupBox1.Visible = true;
					dgridCategory.AllowUserToAddRows = false;
					dgridCategory.EnableHeadersVisualStyles = false;
					dgridCategory.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
					dgridCategory.Columns["SrNo"].DefaultCellStyle.BackColor = Color.LightGray;
					dgridCategory.Columns["SrNo"].Width = 50;
					dgridCategory.Columns["Category"].DefaultCellStyle.BackColor = Color.LightGray;
					dgridCategory.Columns["Discount"].DefaultCellStyle.BackColor = Color.LightGray;
					dgridCategory.Columns["NetPrice"].DefaultCellStyle.BackColor = Color.LightGray;
					dgridCategory.Columns["NoofMembers"].DefaultCellStyle.BackColor = Color.White;
					dgridCategory.Columns["Complimentary"].DefaultCellStyle.BackColor = Color.LightGray;
					dgridCategory.Columns["Amount"].DefaultCellStyle.BackColor = Color.LightGray;
					foreach (DataGridViewColumn column in dgridCategory.Columns)
					{
						column.SortMode = DataGridViewColumnSortMode.NotSortable;
						column.Resizable = DataGridViewTriState.False;
					}
					but_print.Visible = true;
					but_close.Visible = true;
					label2.Visible = true;
					txt_netAmount.Visible = true;
					lbl_paymentMode.Visible = true;
					Cmb_PaymentMode.Visible = true;
					string itemdetails = Common.GetData(string.Format("{0}/api/package/GetPackage", appsetting));
					var itemdetailsGet = JsonConvert.DeserializeObject<List<Common.RootObject>>(itemdetails);
					foreach (var Package in itemdetailsGet)
					{
						if (TextPackage.ToString() == Package.packageName)
						{
							lblpackdetail.Text = Package.itemName;
						}
					}
					GettimingPackage(item.Value.ToString());
					lblpackname.Visible = true;
					lblpackdetail.Visible = true;
					panel2.Visible = true;
					panel1.Visible = true;
					PaymentMethod();
					dgridCategory.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
					dgridCategory.AllowUserToResizeRows = false;
					dgridCategory.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
					dgridCategory.ColumnHeadersHeight = 100;
					dgridCategory.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
				}
			}));
			setDataSource(dt);
		}

		public void PackageCalculation()
		{
			add = 0;
			add1 = 0;
			addTk = 0;
			Totalmembercalculate = 0;
			int calculateComplimentary;
			for (int i = 0; i < dgridCategory.Rows.Count; ++i)
			{
				int NoofMembers = int.Parse(dgridCategory.Rows[i].Cells["NoofMembers"].Value.ToString());
				decimal groupAmount = decimal.Parse(dgridCategory.Rows[i].Cells["groupAmount"].Value.ToString());
				decimal groupDiscount = decimal.Parse(dgridCategory.Rows[i].Cells["groupDiscountAmount"].Value.ToString());
				decimal PackageDiscount = decimal.Parse(dgridCategory.Rows[i].Cells["packageDiscountAmount"].Value.ToString());
				decimal Prices = decimal.Parse(dgridCategory.Rows[i].Cells["Price"].Value.ToString());
				int Complimentary = int.Parse(dgridCategory.Rows[i].Cells["Compli"].Value.ToString());
				int TotalMembers = int.Parse(dgridCategory.Rows[i].Cells["totalMembers"].Value.ToString());
				if (NoofMembers >= TotalMembers)
				{
					bool isgroupcheck = bool.Parse(dgridCategory.Rows[i].Cells["isGroup"].Value.ToString());
					dgridCategory.Rows[i].Cells["NetPrice"].Value = groupAmount - groupDiscount;
					calculateComplimentary = NoofMembers / 25;
					Totalmembercalculate += NoofMembers + calculateComplimentary;
					if (isgroupcheck == true)
					{
						dgridCategory.Rows[i].Cells["Complimentary"].Value = calculateComplimentary;
					}
					else
					{
						dgridCategory.Rows[i].Cells["Complimentary"].Value = 0;
					}
					dgridCategory.Rows[i].Cells[2].Value = groupAmount;
					add = Convert.ToInt32(NoofMembers * Convert.ToDecimal(dgridCategory.Rows[i].Cells["NetPrice"].Value));
				}
				else
				{
					Totalmembercalculate += NoofMembers;
					decimal NetAmount = Convert.ToDecimal(dgridCategory.Rows[i].Cells["NetPrice"].Value);
					if (j == 0)
					{
						dgridCategory.Rows[i].Cells["NetPrice"].Value = NetAmount;
					}
					else
					{
						dgridCategory.Rows[i].Cells["NetPrice"].Value = Prices - PackageDiscount;
					}
					dgridCategory.Rows[i].Cells["Complimentary"].Value = 0;
					dgridCategory.Rows[i].Cells[2].Value = Prices;
					add = Convert.ToInt32(NoofMembers * Convert.ToDecimal(dgridCategory.Rows[i].Cells["NetPrice"].Value));
				}
				dgridCategory.Rows[i].Cells["Amount"].Value = add.ToString();
				totalticketsCount += NoofMembers;
				add1 += add;
				totalticketsCount += totalticketsCount;
				txt_netAmount.Text = "₹ " + add1.ToString() + " /-";
				txtTT.Text = "₹ " + add1.ToString() + " /-";
				FinalTotalCash = add1;
				j++;
			}
			if (StartBooking.StaffFk != null)
			{
				txt_netAmount.Text = "₹ " + "0.00" + " /-";
			}
		}

		public void SubmitPackage()
		{
			bool isChecked = rbd_Package.Checked;
			if (isChecked == true)
			{
				check = true;
			}
			else
			{
				check = false;
			}
			if (StartBooking.StaffFk != null)
			{
				txtTT.Text = "0";
			}
			int RequestIdWindow = rnd.Next(10000000, 99999999);
			string Paymentvalues = ((KeyValuePair<string, string>)Cmb_PaymentMode.SelectedItem).Value;
			string json = new JavaScriptSerializer().Serialize(new
			{
				FirstName = txt_FirstName.Text,
				LastName = txt_LastName.Text,
				MobileNumber = txt_MobileNumber.Text,
				Email = txtemail.Text,
				TotalAmount = txtTT.Text.Replace("₹", "").Replace("/-", ""),
				UserId = UserFk,
				isPakcageTicket = check,
				PaymentMode = Paymentvalues,
				Miscellaneous = MislaneousCharges,
				Dateofvisits = txt_visitdate.Text,
				RequestId = RequestIdWindow,
				CounterFK = LoginControl.CounterFk,
				BookedBy = LoginControl.ID,
				CouponFk = CouponFk
			});
			var serializer = new JavaScriptSerializer();
			string data = Common.PostMethod(json, string.Format("{0}/api/Ticket/SaveTicketWindow", appsetting));
			dynamic jsonObject = serializer.Deserialize<dynamic>(data);
			string _key = jsonObject["status"];
			string TicketId = jsonObject["userID"];
			Tkt_Id = TicketId;
			string discountAmount;
			if (_key == "1")
			{
				string myidsnew = dgridCategory.Rows[0].Cells["itemid"].Value.ToString();
				if (myidsnew.ToString() != "")
				{
					if (isChecked == true)
					{
						PackageFK = myidsnew;
						itemFk = null;
					}
					else
					{
						itemFk = myidsnew;
						PackageFK = null;
					}
					jsonTicketPackage = new JavaScriptSerializer().Serialize(new
					{
						TicketFK = TicketId,
						itemFk = itemFk,
						PackageFK = PackageFK
					});
					var serializerTicketPackage = new JavaScriptSerializer();
					string dataTicketPackage = Common.PostMethod(jsonTicketPackage, string.Format("{0}/api/Ticket/SaveTicketPackageWindow", appsetting));
					dynamic jsonObjectTicketPackage = serializerTicketPackage.Deserialize<dynamic>(dataTicketPackage);
					string _key1 = jsonObjectTicketPackage["status"];
					TicketPackageId = jsonObjectTicketPackage["userID"];
					for (int i = 0; i < dgridCategory.Rows.Count; ++i)
					{
						int NoofPersaonsTotal = int.Parse(dgridCategory.Rows[i].Cells["NoofMembers"].Value.ToString());
						string NetAmount = dgridCategory.Rows[i].Cells["NetPrice"].Value.ToString();
						string TotalFinal = dgridCategory.Rows[i].Cells["Amount"].Value.ToString();
						int discount = Convert.ToInt32(dgridCategory.Rows[i].Cells["Discount"].Value.ToString());
						string Priceper = dgridCategory.Rows[i].Cells[2].Value.ToString();
						int Complimentary = int.Parse(dgridCategory.Rows[i].Cells["Complimentary"].Value.ToString());
						string CategoryidPackageid = dgridCategory.Rows[i].Cells["CategoryidPackage"].Value.ToString();
						bool isGroup = Convert.ToBoolean(dgridCategory.Rows[i].Cells["isGroup"].Value.ToString());
						int TotalMembers = int.Parse(dgridCategory.Rows[i].Cells["totalMembers"].Value.ToString());
						int NoofAdult = int.Parse(dgridCategory.Rows[i].Cells["NoofAdults"].Value.ToString());
						int NoofMinors = int.Parse(dgridCategory.Rows[i].Cells["NoofMinors"].Value.ToString());
						if (NoofPersaonsTotal >= TotalMembers)
						{
							discountAmount = dgridCategory.Rows[i].Cells["groupDiscountAmount"].Value.ToString();
						}
						else
						{
							discountAmount = dgridCategory.Rows[i].Cells["packageDiscountAmount"].Value.ToString();
						}
						if (NoofPersaonsTotal > 0)
						{
							itemFk = null;
							jsonTicketPackageCategory = new JavaScriptSerializer().Serialize(new
							{
								TicketPackageFK = TicketPackageId,
								CategoryFk = CategoryidPackageid,
								NumberOfTickets = NoofPersaonsTotal.ToString(),
								Price = Priceper.ToString(),
								Discount = discount,
								DiscountAmount = discountAmount,
								SoldPrice = NetAmount.ToString(),
								TotalPrice = TotalFinal.ToString(),
								NumberOfComplimentary = Complimentary.ToString(),
								isGroup = isGroup.ToString(),
								ItemFK = itemFk,
								PackageFK = myidsnew,
								TicketFk = Tkt_Id,
								NumberOfAdult = NoofPersaonsTotal * NoofAdult,
								NumberOfMinor = NoofPersaonsTotal * NoofMinors
							});
							var serializerTicketPackageCategory = new JavaScriptSerializer();
							string dataTicketPackageCategory = Common.PostMethod(jsonTicketPackageCategory, string.Format("{0}/api/Ticket/SaveTicketPackageCategoryWindow", appsetting));
							dynamic jsonObjectTicketPackageCategory = serializerTicketPackageCategory.Deserialize<dynamic>(dataTicketPackageCategory);
							string _keycategory = jsonObjectTicketPackageCategory["status"];
						}
					}
					if (newBox != null)
					{
						string[] s = EnterTimming.Cast<string>().ToArray();
						string[] q = s.Distinct().ToArray();
						foreach (var newtimmingitem in q)
						{
							jsonTicketPackageTimming = new JavaScriptSerializer().Serialize(new
							{
								TicketFK = TicketId,
								TimmingFk = newtimmingitem.ToString()
							});
							var serializerTicketPackageTimming = new JavaScriptSerializer();
							string dataTicketPackageTimming = Common.PostMethod(jsonTicketPackageTimming, string.Format("{0}/api/Ticket/SaveTicketPackageTimmingWindow", appsetting));
							dynamic jsonObjectTicketPackageTimming = serializerTicketPackageTimming.Deserialize<dynamic>(dataTicketPackageTimming);
							string _keyTimming = jsonObjectTicketPackageTimming["status"];
						}
					}
				}
			}
			MessageBox.Show("Item Saved Successfully.");
			Reset();
			valueFalse();
			PaymentStatus = Cmb_PaymentMode.Text;
			but_prints.Visible = true;
			if (Paymentvalues.ToString() == PaymentMode.CARD.ToString())
			{
				but_mislaneous.Visible = true;
			}
		}

		//Tickets

		public void GetTickets()
		{
			int ck = 0;
			ThirdPartyLogin.Clear();
			var ticketsTime = ht.time(DateTime.Parse(DateTime.Now.ToString()));
			lst_Package.Items.Clear();
			string data = Common.GetData(string.Format("{0}/api/Item/GetItems?Date={1}&UserId={2}", appsetting, ticketsTime, LoginControl.ID));
			var jsonObject = JsonConvert.DeserializeObject<Common.RootObject_Tickets[]>(data);
			if (jsonObject != null)
			{
				foreach (var tickets in jsonObject)
				{
					ComboboxItem item = new ComboboxItem();
					if (tickets.disabled == true)
					{
						item.Text = tickets.text;
						item.Value = tickets.value;
						item.tagvalue = tickets.Description;
						item.isselected = tickets.ThirdParty;
						lst_Package.Items.Add(item, CheckState.Indeterminate);
						lst_Package.ItemCheck += (s, e) => { if (e.CurrentValue == CheckState.Indeterminate) e.NewValue = CheckState.Indeterminate; };
					}
					else
					{
						item.Text = tickets.text;
						item.Value = tickets.value;
						item.tagvalue = tickets.Description;
						item.isselected = tickets.ThirdParty;
						lst_Package.Items.Add(item, CheckState.Unchecked);
					}
					ck++;
				}
			}
		}

		public void TicketsGrid()
		{
			this.Invoke(new MethodInvoker(delegate
			{
				//Control.CheckForIllegalCrossThreadCalls = false;
				int i2 = 0;
				var ticketsTime = ht.time(DateTime.Parse(DateTime.Now.ToString()));
				dgridCategory.ClearSelection();
				dgridCategory.Columns.Clear();
				int i1 = 1;
				dt.Columns.Clear();
				dt.Rows.Clear();
				dr = dt.NewRow();
				dt.Columns.Add("SrNo");
				dt.Columns.Add("Category");
				dt.Columns.Add("Categoryid");
				dt.Columns.Add("totalMembers");
				dt.Columns.Add("isGroup");
				dt.Columns.Add("NoofAdults");
				dt.Columns.Add("NoofMinors");
				if (LoginControl.RoleNames == GetDescription(Role.THIRDPARTY))
				{
					data = Common.GetData(string.Format("{0}/api/Category?IsThirdParty={1}", appsetting, true));
				}
				else
				{
					data = Common.GetData(string.Format("{0}/api/Category", appsetting));
				}
				var jsonObject = JsonConvert.DeserializeObject<List<Common.Category>>(data);
				foreach (Common.ComboboxItem item in lst_Package.CheckedItems)
				{
					if (item.isselected == true)
					{
						ThirdPartyLogin.Add(item);
					}
				}
				foreach (var categories in jsonObject)
				{
					dr = dt.NewRow();
					dr["SrNo"] = i1;
					dr["Category"] = categories.categoryName;
					dr["Categoryid"] = categories.id;
					dr["totalMembers"] = categories.totalMembers;
					dr["isGroup"] = categories.isGroup;
					dr["NoofAdults"] = categories.NoofAdults;
					dr["NoofMinors"] = categories.NoofMinors;
					foreach (Common.ComboboxItem item in lst_Package.CheckedItems)
					{
						var propertyInfo = item.GetType().GetProperty("tagvalue");
						var TextPackage = propertyInfo.GetValue(item, null);
						if (i2 == 0)
						{
							dt.Columns.Add(TextPackage.ToString());
							dt.Columns.Add("Discount" + ht.Removespace(TextPackage.ToString()));
							dt.Columns.Add("tax" + ht.Removespace(TextPackage.ToString()));
							dt.Columns.Add("NetPrice" + ht.Removespace(TextPackage.ToString()));
							dt.Columns.Add("Amount" + ht.Removespace(TextPackage.ToString()));
							dt.Columns.Add("itemid" + ht.Removespace(TextPackage.ToString()));
							lsttimming.Add(item.Value);
						}
						Path = string.Format("{0}/api/Item/GetItemPrice?ItemFK={1}&CategoryFK={2}&Date={3}", appsetting, item.Value, categories.id, ticketsTime);
						string Amount = Common.GetData(Path);
						var jsonObject1 = JsonConvert.DeserializeObject<Common.RootObject_NewPrice>(Amount);
						dr[TextPackage.ToString()] = jsonObject1.soldPrice;
						dr["Discount" + ht.Removespace(TextPackage.ToString())] = Convert.ToInt32(jsonObject1.discountDetail.value.discount);
						dr["tax" + ht.Removespace(TextPackage.ToString())] = Convert.ToInt32(jsonObject1.discountDetail.value.amount);
						dr["NetPrice" + ht.Removespace(TextPackage.ToString())] = (jsonObject1.soldPrice - jsonObject1.discountDetail.value.amount);
						dr["Amount" + ht.Removespace(TextPackage.ToString())] = 0;
						dr["itemid" + ht.Removespace(TextPackage.ToString())] = item.Value;
					}
					i1++;
					i2++;
					dt.Rows.Add(dr);
				}
				dt.Columns.Add("NoofMembers");
				dt.Columns.Add("Complimentary");
				dt.Columns.Add("Total");
				dgridCategory.DataSource = dt;
				for (int i = 0; i < dgridCategory.Rows.Count; ++i)
				{
					dgridCategory.Rows[i].Cells[dgridCategory.Columns.Count - 3].Value = 0;
					dgridCategory.Rows[i].Cells[dgridCategory.Columns.Count - 2].Value = 0;
					dgridCategory.Rows[i].Cells[dgridCategory.Columns.Count - 1].Value = 0;
				}
				for (int i = 0; i < dgridCategory.Columns.Count; ++i)
				{
					dgridCategory.Columns[i].ReadOnly = true;
				}
				dgridCategory.Columns[dgridCategory.Columns.Count - 3].ReadOnly = false;
				groupBox1.Visible = true;
				dgridCategory.AllowUserToAddRows = false;
				dgridCategory.EnableHeadersVisualStyles = false;
				dgridCategory.Columns["Categoryid"].Visible = false;
				dgridCategory.Columns["totalMembers"].Visible = false;
				dgridCategory.Columns["isGroup"].Visible = false;
				dgridCategory.Columns["NoofAdults"].Visible = false;
				dgridCategory.Columns["NoofMinors"].Visible = false;
				foreach (Common.ComboboxItem item in lst_Package.CheckedItems)
				{
					var propertyInfo = item.GetType().GetProperty("tagvalue");
					var TextPackage = propertyInfo.GetValue(item, null);
					dgridCategory.Columns["tax" + ht.Removespace(TextPackage.ToString())].Visible = false;
					dgridCategory.Columns["itemid" + ht.Removespace(TextPackage.ToString())].Visible = false;
					dgridCategory.Columns["Discount" + ht.Removespace(TextPackage.ToString())].DefaultCellStyle.BackColor = Color.LightGray;
					dgridCategory.Columns["NetPrice" + ht.Removespace(TextPackage.ToString())].DefaultCellStyle.BackColor = Color.LightGray;
					dgridCategory.Columns["Amount" + ht.Removespace(TextPackage.ToString())].DefaultCellStyle.BackColor = Color.LightGray;
				}
				dgridCategory.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
				dgridCategory.Columns["SrNo"].DefaultCellStyle.BackColor = Color.LightGray;
				dgridCategory.Columns["Category"].DefaultCellStyle.BackColor = Color.LightGray;
				dgridCategory.Columns["Total"].DefaultCellStyle.BackColor = Color.LightGray;
				dgridCategory.Columns["Complimentary"].DefaultCellStyle.BackColor = Color.LightGray;
				dgridCategory.Columns["SrNo"].Width = 50;
				dgridCategory.Columns["SrNo"].Frozen = true;
				dgridCategory.Columns["Category"].Frozen = true;
				foreach (DataGridViewColumn column in dgridCategory.Columns)
				{
					column.SortMode = DataGridViewColumnSortMode.NotSortable;
					column.Resizable = DataGridViewTriState.False;
				}
				GettimingTickets();
				panel2.Visible = true;
				panel1.Visible = true;
				but_print.Visible = true;
				but_close.Visible = true;
				label2.Visible = true;
				txt_netAmount.Visible = true;
				lbl_paymentMode.Visible = true;
				Cmb_PaymentMode.Visible = true;
				PaymentMethod();
				dgridCategory.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
				dgridCategory.AllowUserToResizeRows = false;
				dgridCategory.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
				dgridCategory.ColumnHeadersHeight = 100;
				dgridCategory.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			}));
			setDataSource(dt);
		}

		public void TicketsGridCalculation()
		{
			add = 0;
			add1 = 0;
			addTk = 0;
			Totalmembercalculate = 0;
			int calculateComplimentary;
			for (int i = 0; i < dgridCategory.Rows.Count; ++i)
			{
				add = 0;
				addTk = 0;
				int NoofMembers = int.Parse(dgridCategory.Rows[i].Cells["NoofMembers"].Value.ToString());
				int TotalMembers = int.Parse(dgridCategory.Rows[i].Cells["totalMembers"].Value.ToString());
				int Comlimentarry = int.Parse(dgridCategory.Rows[i].Cells["Complimentary"].Value.ToString());
				Totalmembercalculate += NoofMembers;
				for (int j = 0; j < dgridCategory.Columns.Count; ++j)
				{
					string headertext = dgridCategory.Columns[j].HeaderText;
					if (headertext.ToString().Contains("NetPrice"))
					{
						decimal NeTAmount = decimal.Parse(dgridCategory.Rows[i].Cells[headertext].Value.ToString());
						Amount = Convert.ToInt32(NeTAmount * NoofMembers);
						add += Convert.ToInt32(Amount.ToString());
					}
					if (headertext.ToString().Contains("Amount"))
					{
						dgridCategory.Rows[i].Cells[headertext].Value = Amount;
					}
					if (headertext.ToString().Contains("Total"))
					{
						dgridCategory.Rows[i].Cells[headertext].Value = add;
						addTk += add;
					}
				}
				if (NoofMembers >= TotalMembers)
				{
					bool isgroupcheck = bool.Parse(dgridCategory.Rows[i].Cells["isGroup"].Value.ToString());
					calculateComplimentary = NoofMembers / 25;
					Totalmembercalculate += NoofMembers + calculateComplimentary;
					if (isgroupcheck == true)
					{
						dgridCategory.Rows[i].Cells["Complimentary"].Value = calculateComplimentary;
					}
					else
					{
						dgridCategory.Rows[i].Cells["Complimentary"].Value = 0;
					}
				}
				else
				{
					dgridCategory.Rows[i].Cells["Complimentary"].Value = 0;
				}
				add1 += addTk;
				txt_netAmount.Text = "₹ " + add1.ToString() + " /-";
				txtTT.Text = "₹ " + add1.ToString() + " /-";
				FinalTotalCash = add1;
			}
			if (StartBooking.StaffFk != null)
			{
				txt_netAmount.Text = "₹ " + "0.00" + " /-";
			}
		}

		public void SubmitTickets()
		{
			bool isChecked = rbd_Package.Checked;
			if (isChecked == true)
			{
				check = true;
			}
			else
			{
				check = false;
			}
			string Paymentvalues = ((KeyValuePair<string, string>)Cmb_PaymentMode.SelectedItem).Value;
			if (StartBooking.StaffFk != null)
			{
				txtTT.Text = "0";
			}
			int RequestIdWindow = rnd.Next(10000000, 99999999);
			string json = new JavaScriptSerializer().Serialize(new
			{
				FirstName = txt_FirstName.Text,
				LastName = txt_LastName.Text,
				MobileNumber = txt_MobileNumber.Text,
				Email = txtemail.Text,
				TotalAmount = txtTT.Text.Replace("₹", "").Replace("/-", ""),
				UserId = UserFk,
				isPakcageTicket = check,
				PaymentMode = Paymentvalues,
				miscellaneous = MislaneousCharges,
				Dateofvisits = txt_visitdate.Text,
				RequestId = RequestIdWindow,
				CounterFK = LoginControl.CounterFk,
				BookedBy = LoginControl.ID,
				CouponFk = CouponFk
			});
			var serializer = new JavaScriptSerializer();
			string data = Common.PostMethod(json, string.Format("{0}/api/Ticket/SaveTicketWindow", appsetting));
			dynamic jsonObject = serializer.Deserialize<dynamic>(data);
			string _key = jsonObject["status"];
			string TicketId = jsonObject["userID"];
			Tkt_Id = TicketId;
			if (_key == "1")
			{
				foreach (Common.ComboboxItem item in lst_Package.CheckedItems)
				{
					maxcouintitem = lst_Package.CheckedItems.Count;
					string myidsnew = item.Value.ToString();
					if (myidsnew.ToString() != "")
					{
						if (isChecked == true)
						{
							PackageFK = myidsnew;
							itemFk = null;
						}
						else
						{
							itemFk = myidsnew;
							PackageFK = null;
						}
						jsonTicketPackage = new JavaScriptSerializer().Serialize(new
						{
							TicketFK = TicketId,
							itemFk = itemFk,
							PackageFK = PackageFK,
						});
						var serializerTicketPackage = new JavaScriptSerializer();
						string dataTicketPackage = Common.PostMethod(jsonTicketPackage, string.Format("{0}/api/Ticket/SaveTicketPackageWindow", appsetting));
						dynamic jsonObjectTicketPackage = serializerTicketPackage.Deserialize<dynamic>(dataTicketPackage);
						string _key1 = jsonObjectTicketPackage["status"];
						TicketPackageId = jsonObjectTicketPackage["userID"];
					}
				}
			}
			for (int i = 0; i < dgridCategory.Rows.Count; ++i)
			{
				int NoofMembers = int.Parse(dgridCategory.Rows[i].Cells["NoofMembers"].Value.ToString());
				int Noofcomplimentary = int.Parse(dgridCategory.Rows[i].Cells["Complimentary"].Value.ToString());
				string Categoryid = dgridCategory.Rows[i].Cells["Categoryid"].Value.ToString();
				int NoofAdult = int.Parse(dgridCategory.Rows[i].Cells["NoofAdults"].Value.ToString());
				int NoofMinors = int.Parse(dgridCategory.Rows[i].Cells["NoofMinors"].Value.ToString());
				if (NoofMembers > 0)
				{
					int getfinalcount = 0;
					for (int j = 0; j < dgridCategory.Columns.Count; ++j)
					{
						countcolumns = dgridCategory.Columns.Count;
						if (getfinalcount <= 12 && countvalues == false)
						{
							string headertext = dgridCategory.Columns[j].HeaderText;
							if (headertext.ToString().Contains("NetPrice"))
							{
								NeTAmountTk = decimal.Parse(dgridCategory.Rows[i].Cells[headertext].Value.ToString());
							}
							if (headertext.ToString().Contains("Amount"))
							{
								Amount = int.Parse(dgridCategory.Rows[i].Cells[headertext].Value.ToString());
							}
							if (headertext.ToString().Contains("Discount"))
							{
								DiscountTk = int.Parse(dgridCategory.Rows[i].Cells[headertext].Value.ToString());
							}
							if (headertext.ToString().Contains("tax"))
							{
								discountAmountTk = int.Parse(dgridCategory.Rows[i].Cells[headertext].Value.ToString());
							}
							if (headertext.ToString().Contains("itemid"))
							{
								itemidTickets = dgridCategory.Rows[i].Cells[headertext].Value.ToString();
								Path = string.Format("{0}/api/Ticket/GetTicketPackage?TicketFK={1}&ItemFK={2}&PackageFK={3}", appsetting, TicketId, itemidTickets, null);
								string DetailTictetpackage = Common.GetData(Path);
								var jsonObject1_tickate = JsonConvert.DeserializeObject<Common.RootObject_TickatePackage>(DetailTictetpackage);
								FindTicketsid = jsonObject1_tickate.id;
							}
							getfinalcount++;
							if (getfinalcount == 13)
							{
								FinalPricePer = Convert.ToInt32(NeTAmountTk + discountAmountTk);
								countvalues = true;
								jsonTicketPackageCategory = new JavaScriptSerializer().Serialize(new
								{
									TicketPackageFK = FindTicketsid,
									CategoryFk = Categoryid,
									NumberOfTickets = NoofMembers.ToString(),
									Price = FinalPricePer.ToString(),
									Discount = DiscountTk.ToString(),
									DiscountAmount = discountAmountTk.ToString(),
									SoldPrice = NeTAmountTk.ToString(),
									TotalPrice = Amount.ToString(),
									NumberOfComplimentary = Noofcomplimentary,
									isGroup = false,
									NumberOfAdult = NoofMembers * NoofAdult,
									NumberOfMinor = NoofMembers * NoofMinors
								});
								var serializerTicketPackageCategory = new JavaScriptSerializer();
								string dataTicketPackageCategory = Common.PostMethod(jsonTicketPackageCategory, string.Format("{0}/api/Ticket/SaveTicketPackageCategoryWindow", appsetting));
								dynamic jsonObjectTicketPackageCategory = serializerTicketPackageCategory.Deserialize<dynamic>(dataTicketPackageCategory);
								string _keycategory = jsonObjectTicketPackageCategory["status"];
								getfinalcount = 0;
							}
						}
						else if (getfinalcount <= 6 && countvalues == true && j <= countcolumns)
						{
							string headertext = dgridCategory.Columns[j].HeaderText;
							if (headertext.ToString().Contains("NetPrice"))
							{
								NeTAmountTk = decimal.Parse(dgridCategory.Rows[i].Cells[headertext].Value.ToString());
							}
							if (headertext.ToString().Contains("Amount"))
							{
								Amount = int.Parse(dgridCategory.Rows[i].Cells[headertext].Value.ToString());
							}
							if (headertext.ToString().Contains("Discount"))
							{
								DiscountTk = int.Parse(dgridCategory.Rows[i].Cells[headertext].Value.ToString());
							}
							if (headertext.ToString().Contains("tax"))
							{
								discountAmountTk = int.Parse(dgridCategory.Rows[i].Cells[headertext].Value.ToString());
							}
							if (headertext.ToString().Contains("itemid"))
							{
								itemidTickets = dgridCategory.Rows[i].Cells[headertext].Value.ToString();
								Path = string.Format("{0}/api/Ticket/GetTicketPackage?TicketFK={1}&ItemFK={2}&PackageFK={3}", appsetting, TicketId, itemidTickets, null);
								string DetailTictetpackage = Common.GetData(Path);
								var jsonObject1_tickate = JsonConvert.DeserializeObject<Common.RootObject_TickatePackage>(DetailTictetpackage);
								FindTicketsid = jsonObject1_tickate.id;
							}
							getfinalcount++;
							if (getfinalcount == 6)
							{
								FinalPricePer = Convert.ToInt32(NeTAmountTk + discountAmountTk);
								countvalues = true;
								jsonTicketPackageCategory = new JavaScriptSerializer().Serialize(new
								{
									TicketPackageFK = FindTicketsid,
									CategoryFk = Categoryid,
									NumberOfTickets = NoofMembers.ToString(),
									Price = FinalPricePer.ToString(),
									Discount = DiscountTk.ToString(),
									DiscountAmount = discountAmountTk.ToString(),
									SoldPrice = NeTAmountTk.ToString(),
									TotalPrice = Amount.ToString(),
									NumberOfComplimentary = Noofcomplimentary,
									isGroup = false,
									NumberOfAdult = NoofMembers * NoofAdult,
									NumberOfMinor = NoofMembers * NoofMinors
								});
								var serializerTicketPackageCategory = new JavaScriptSerializer();
								string dataTicketPackageCategory = Common.PostMethod(jsonTicketPackageCategory, string.Format("{0}/api/Ticket/SaveTicketPackageCategoryWindow", appsetting));
								dynamic jsonObjectTicketPackageCategory = serializerTicketPackageCategory.Deserialize<dynamic>(dataTicketPackageCategory);
								string _keycategory = jsonObjectTicketPackageCategory["status"];
								getfinalcount = 0;
								findlstvalues++;
							}
						}
					}
					countvalues = false;
				}
			}
			if (newBox != null)
			{
				string[] s = EnterTimming.Cast<string>().ToArray();
				string[] q = s.Distinct().ToArray();
				foreach (var newtimmingitem in q)
				{
					jsonTicketPackageTimming = new JavaScriptSerializer().Serialize(new
					{
						TicketFK = TicketId,
						TimmingFk = newtimmingitem.ToString()
					});
					var serializerTicketPackageTimming = new JavaScriptSerializer();
					string dataTicketPackageTimming = Common.PostMethod(jsonTicketPackageTimming, string.Format("{0}/api/Ticket/SaveTicketPackageTimmingWindow", appsetting));
					dynamic jsonObjectTicketPackageTimming = serializerTicketPackageTimming.Deserialize<dynamic>(dataTicketPackageTimming);
					string _keyTimming = jsonObjectTicketPackageTimming["status"];
				}
			}
			insertticketdata();
			MessageBox.Show("Item Saved Successfully.");
			Reset();
			valueFalse();
			PaymentStatus = Cmb_PaymentMode.Text;
			but_prints.Visible = true;
			if (Paymentvalues.ToString() == PaymentMode.CARD.ToString())
			{
				but_mislaneous.Visible = true;
			}
		}

		public void insertticketdata()
		{
			foreach (var item in ThirdPartyLogin)
			{
				for (int i = 0; i < dgridCategory.Rows.Count; ++i)
				{
					int NoofMembers = int.Parse(dgridCategory.Rows[i].Cells["NoofMembers"].Value.ToString());
					int Noofcomplimentary = int.Parse(dgridCategory.Rows[i].Cells["Complimentary"].Value.ToString());
					string Categoryid = dgridCategory.Rows[i].Cells["Categoryid"].Value.ToString();
					var propertyInfo = item.GetType().GetProperty("Value");
					var TextPackage = propertyInfo.GetValue(item, null);
					if (NoofMembers > 0)
					{
						jsonTicketPrintdata = new JavaScriptSerializer().Serialize(new
						{
							ItemFk = TextPackage,
							CategoryFK = Categoryid,
							TicketFK = Tkt_Id,
							NumberOfTickets = NoofMembers,
							NumberOfComplimentary = Noofcomplimentary
						});
						var serializerTicketPrintdata = new JavaScriptSerializer();
						string dataTicketPackageCategory = Common.PostMethod(jsonTicketPrintdata, string.Format("{0}/api/Ticket/InsertTicketdata", appsetting));
						dynamic jsonObjectTicketPackageCategory = serializerTicketPrintdata.Deserialize<dynamic>(dataTicketPackageCategory);
						string _keycategory = jsonObjectTicketPackageCategory["status"];
					}
				}
			}
		}

		//Common Method

		public void checkthirdparty()
		{
			ThirdPartyLogin.Clear();
			string data = Common.GetData(string.Format("{0}/api/package/GetPackage?UserId={1}", appsetting, LoginControl.ID));
			var jsonObject = JsonConvert.DeserializeObject<List<Common.RootObject>>(data);
			foreach (Common.ComboboxItem item in lst_Package.CheckedItems)
			{
				foreach (var details in jsonObject)
				{
					string Pacid = item.Value.ToString();
					if (Guid.Parse(Pacid) == details.id)
					{
						string selectpackitem = details.packageItem;
						string Packageitem = Common.GetData(string.Format("{0}/api/Ticket/GetThirdparty?Packageitems={1}", appsetting, selectpackitem));
						RootObject_Thirdparty PackageitemObject = JsonConvert.DeserializeObject<RootObject_Thirdparty>(Packageitem);
						foreach (var itempackage in PackageitemObject.data.Split(','))
						{
							string nt = "/";
							ComboboxItem bindnewvalue = new ComboboxItem();
							int posA = itempackage.IndexOf('/');
							bindnewvalue.Text = itempackage.Substring(0, posA);
							int posb = itempackage.LastIndexOf(nt);
							int adjustedPosA = posb + nt.Length;
							bindnewvalue.Value = itempackage.Substring(adjustedPosA);
							ThirdPartyLogin.Add(bindnewvalue);
						}
					}
				}
			}
		}

		public void ShowUserDetails()
		{
			if (StartBooking.StaffFk != null)
			{
				Path = string.Format("{0}/api/Membership/GetMemberdetails?UserFk={1}", appsetting, StartBooking.StaffFk);
			}
			if (StartBooking.Schoolfk != null)
			{
				Path = string.Format("{0}/api/Membership/GetMemberdetails?UserFk={1}", appsetting, StartBooking.UserFk);
			}
			string DetailTictetpackage = Common.GetData(Path);
			var Memberdetails = JsonConvert.DeserializeObject<Common.RootObject_Memberdetails>(DetailTictetpackage);
			if (Memberdetails != null)
			{
				txt_FirstName.Text = Memberdetails.FirstName;
				txt_LastName.Text = Memberdetails.LastName;
				txtemail.Text = Memberdetails.Email;
				txt_MobileNumber.Text = Memberdetails.MobileNumber;
			}
		}

		public void SubmitTicketSchool()
		{
			string jsonSchoolTicket = new JavaScriptSerializer().Serialize(new
			{
				TicketFK = Tkt_Id,
				SchoolFk = StartBooking.Schoolfk
			});
			var serializerSchoolTicket = new JavaScriptSerializer();
			string dataSchoolTicket = Common.PostMethod(jsonSchoolTicket, string.Format("{0}/api/School/CreateTicketSchool", appsetting));
			dynamic jsonObjectSchoolTicket = serializerSchoolTicket.Deserialize<dynamic>(dataSchoolTicket);
			txtemail.ReadOnly = false;
		}

		private void PaymentMethod()
		{
			Dictionary<string, string> test = new Dictionary<string, string>();
			test.Add(((int)PaymentMode.CASH).ToString(), PaymentMode.CASH.ToString());
			test.Add(((int)PaymentMode.CARD).ToString(), PaymentMode.CARD.ToString());
			Cmb_PaymentMode.DataSource = new BindingSource(test, null);
			Cmb_PaymentMode.DisplayMember = "Value";
			Cmb_PaymentMode.ValueMember = "Key";
			Cmb_PaymentMode.SelectedIndex = 0;
		}

		public void valueFalse()
		{
			but_print.Visible = false;
			but_close.Visible = false;
			label2.Visible = false;
			txt_netAmount.Visible = false;
			lbl_paymentMode.Visible = false;
			Cmb_PaymentMode.Visible = false;
			dgridCategory.DataSource = null;
			groupBox1.Visible = false;
			txt_netAmount.Text = string.Empty;
			lblpackname.Visible = false;
			lblpackdetail.Visible = false;
			panel2.Visible = false;
			panel1.Visible = false;
			countvalues = false;
			txt_mislaneous.Visible = false;
		}

		public void Reset()
		{
			txt_FirstName.Text = string.Empty;
			txt_MobileNumber.Text = string.Empty;
			txt_LastName.Text = string.Empty;
			txtemail.Text = string.Empty;
		}

		//Timming method

		////Get Package timmimg 
		public List<Common.ComboboxItem> GettimingPackage(string pkd_id)
		{
			List<Common.ComboboxItem> cmd = new List<Common.ComboboxItem>();
			this.Invoke(new MethodInvoker(delegate
			{
				panel1.Controls.Clear();
				panel2.Controls.Clear();
				var timeformat = DateTime.Parse(DateTime.Now.ToString());
				var t = ht.time(timeformat);
				Path = string.Format("{0}/api/Package/GetTimming?PackageFK={1}&strDate={2}&Mode={3}", appsetting, pkd_id, t, "Offline");
				string data = Common.GetData(Path);
				var jsonObject_Timimg = JsonConvert.DeserializeObject<Common.RootObject12>(data);
				countotal = jsonObject_Timimg.timming.Count;
				if (jsonObject_Timimg.timming.Count > 0)
				{
					for (int i = 0; i < countotal; ++i)
					{
						cmb = new ComboBox[15];
						lb = new Label[15];
						int width = 90;
						int height = 25;
						int spacing = 5;
						newBox = new ComboBox();
						newBox.SelectedIndexChanged -= NewBox_SelectedIndexChanged;
						newBox.SelectedIndexChanged += NewBox_SelectedIndexChanged;
						Label lb1 = new Label();
						foreach (var time in jsonObject_Timimg.timming[i].timmings)
						{
							Common.ComboboxItem cmbitem = new Common.ComboboxItem { Text = time.text, Value = time.value };
							newBox.Items.Add(cmbitem);
							lb1.Text = jsonObject_Timimg.timming[i].itemName;
							newBox.Size = new Size(width, height);
							newBox.Location = new Point((i * width) + spacing, 0);
							lb1.Size = new Size(width, height);
							lb1.Location = new Point((i * width) + spacing, 0);
							cmb[i] = newBox;
							lb[i] = lb1;
							panel1.Controls.Add(newBox);
							panel2.Controls.Add(lb1);
							createId++;
						}
						newBox.Items.Insert(0, "--Select--");
						newBox.SelectedIndex = 0;
					}
				}
			}));
			return cmd;
		}

		//Get Ticket timmimg 
		public List<Common.ComboboxItem> GettimingTickets()
		{
			panel1.Controls.Clear();
			panel2.Controls.Clear();
			List<Common.ComboboxItem> cmd = new List<Common.ComboboxItem>();
			foreach (var items in lsttimming)
			{
				var timeformat = DateTime.Parse(DateTime.Now.ToString());
				var t = ht.time(timeformat);
				Path = string.Format("{0}/api/item/GetTimming?ItemFK={1}&strDate={2}&Mode={3}", appsetting, items.ToString(), t, "Offline");
				string data = Common.GetData(Path);
				var jsonObject_Timimg = JsonConvert.DeserializeObject<Common.RootObject12>(data);
				countotal = jsonObject_Timimg.timming.Count;
				if (jsonObject_Timimg.timming.Count > 0)
				{
					for (int i = 0; i < countotal; ++i)
					{
						cmb = new ComboBox[15];
						lb = new Label[15];
						int width = 90;
						int height = 25;
						int spacing = 5;
						newBox = new ComboBox();
						newBox.SelectedIndexChanged -= NewBox_SelectedIndexChanged;
						newBox.SelectedIndexChanged += NewBox_SelectedIndexChanged;
						Label lb1 = new Label();
						foreach (var time in jsonObject_Timimg.timming[i].timmings)
						{
							Common.ComboboxItem cmbitem = new Common.ComboboxItem { Text = time.text, Value = time.value };
							newBox.Items.Add(cmbitem);
							lb1.Text = jsonObject_Timimg.timming[i].itemName;
							newBox.Size = new Size(width, height);
							newBox.Location = new Point((i * width) + spacing, 0);
							lb1.Size = new Size(width, height);
							lb1.Location = new Point((i * width) + spacing, 0);
							cmb[i] = newBox;
							lb[i] = lb1;
							panel1.Controls.Add(newBox);
							panel2.Controls.Add(lb1);
						}
						newBox.Items.Insert(0, "--Select--");
						newBox.SelectedIndex = 0;
					}
				}
			}
			return cmd;
		}

		//Loading Bar Method

		internal delegate void SetDataSourceDelegate(DataTable table);
		private void setDataSource(DataTable table)
		{
			// Invoke method if required:
			if (this.InvokeRequired)
			{
				//dgridCategory.Invoke(new SetDataSourceDelegate(setDataSource), table);
				this.Invoke(new SetDataSourceDelegate(setDataSource), table);
			}
			else
			{
				dgridCategory.DataSource = table;
				progressBar1.Visible = false;
			}
		}

		#endregion

		#region EVENTS

		private void NewBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			newBox = sender as ComboBox;
			int selectedIndex = ((ComboBox)sender).SelectedIndex;
			if (selectedIndex > 0)
			{
				newBox.SelectedItem = ((Common.ComboboxItem)newBox.Items[selectedIndex]);
				var propertyInfo = newBox.SelectedItem.GetType().GetProperty("Value");
				var TextPackage = propertyInfo.GetValue(newBox.SelectedItem, null);
				EnterTimming.Add(TextPackage);
			}
		}

		private void but_close_Click(object sender, EventArgs e)
		{
			this.Hide();
			this.Dispose();
		}

		private void but_print_Click(object sender, EventArgs e)
		{
			int Third = 0;
			errorProvider1.SetError(txtemail, "");
			errorProvider1.SetError(txt_MobileNumber, "");
			bool isChecked = rbd_Package.Checked;
			if (!string.IsNullOrEmpty(txt_MobileNumber.Text) && !string.IsNullOrEmpty(Cmb_PaymentMode.Text) && !string.IsNullOrEmpty(txt_netAmount.Text.ToString()) && !string.IsNullOrEmpty(txt_visitdate.Text.ToString()))
			{
				if (newBox != null)
				{
					errorProvider1.Clear();
					if (newBox.SelectedIndex > 0)
					{
						if (isChecked == true)
						{
							if (StartBooking.StaffFk != null || StartBooking.Schoolfk != null)
							{
								if (StartBooking.Noofmembers <= Totalmembercalculate)
								{
									SubmitPackage();
									foreach (var checkitems in ThirdPartyLogin)
									{
										Button newButton = new Button();
										newButton.Location = new Point(20, 30 * Third + 10);
										newButton.BackColor = Color.LightGray;
										newButton.TextAlign = ContentAlignment.MiddleCenter;
										var propertyInfo = checkitems.GetType().GetProperty("Value");
										var TextPackage = propertyInfo.GetValue(checkitems, null);
										newButton.Tag = TextPackage;
										newButton.Text = checkitems.ToString();
										panelthirdparty.Controls.Add(newButton);
										newButton.Click += new EventHandler(this.DynamicButton_Click);
										Third++;
									}
									ThirdPartyLogin.Clear();
								}
								else
								{
									MessageBox.Show("Invalid! Your Approved Member Count is " + StartBooking.Noofmembers);
								}
							}
							else
							{
								SubmitPackage();
								foreach (var checkitems in ThirdPartyLogin)
								{
									Button newButton = new Button();
									newButton.Location = new Point(20, 30 * Third + 10);
									newButton.BackColor = Color.LightGray;
									newButton.TextAlign = ContentAlignment.MiddleCenter;
									var propertyInfo = checkitems.GetType().GetProperty("Value");
									var TextPackage = propertyInfo.GetValue(checkitems, null);
									newButton.Tag = TextPackage;
									newButton.Text = checkitems.ToString();
									panelthirdparty.Controls.Add(newButton);
									newButton.Click += new EventHandler(this.DynamicButton_Click);
									Third++;
								}
								ThirdPartyLogin.Clear();
							}
						}
						else
						{
							if (StartBooking.StaffFk != null || StartBooking.Schoolfk != null)
							{
								if (StartBooking.Noofmembers <= Totalmembercalculate)
								{
									SubmitTickets();
									foreach (var checkitems in ThirdPartyLogin)
									{
										Button newButton = new Button();
										newButton.Location = new Point(20, 30 * Third + 10);
										newButton.BackColor = Color.LightGray;
										newButton.TextAlign = ContentAlignment.MiddleCenter;
										var propertyInfo = checkitems.GetType().GetProperty("Value");
										var TextPackage = propertyInfo.GetValue(checkitems, null);
										var propertyInfo1 = checkitems.GetType().GetProperty("tagvalue");
										var TextPackage1 = propertyInfo1.GetValue(checkitems, null);
										newButton.Tag = TextPackage;
										newButton.Text = TextPackage1.ToString();
										panelthirdparty.Controls.Add(newButton);
										newButton.Click += new EventHandler(this.DynamicButton_Click);
										Third++;
									}
									ThirdPartyLogin.Clear();
								}
								else
								{
									MessageBox.Show("Invalid! Your Approved Member Count is " + StartBooking.Noofmembers);
								}
							}
							else
							{
								SubmitTickets();
								foreach (var checkitems in ThirdPartyLogin)
								{
									Button newButton = new Button();
									newButton.Location = new Point(20, 30 * Third + 10);
									newButton.BackColor = Color.LightGray;
									newButton.TextAlign = ContentAlignment.MiddleCenter;
									var propertyInfo = checkitems.GetType().GetProperty("Value");
									var TextPackage = propertyInfo.GetValue(checkitems, null);
									var propertyInfo1 = checkitems.GetType().GetProperty("tagvalue");
									var TextPackage1 = propertyInfo1.GetValue(checkitems, null);
									newButton.Tag = TextPackage;
									newButton.Text = TextPackage1.ToString();
									panelthirdparty.Controls.Add(newButton);
									newButton.Click += new EventHandler(this.DynamicButton_Click);
									Third++;
								}
								ThirdPartyLogin.Clear();
							}
						}
						if (StartBooking.Noofmembers >= Totalmembercalculate)
						{
							if (StartBooking.Schoolfk != null)
							{
								SubmitTicketSchool();
							}
						}
						EnterTimming.Clear();
					}
					else
					{
						MessageBox.Show("Please select the Show Timming");
					}
				}
				else
				{
					errorProvider1.Clear();
					if (isChecked == true)
					{
						if (StartBooking.StaffFk != null || StartBooking.Schoolfk != null)
						{
							if (StartBooking.Noofmembers <= Totalmembercalculate)
							{
								SubmitPackage();
								foreach (var checkitems in ThirdPartyLogin)
								{
									Button newButton = new Button();
									newButton.Location = new Point(20, 30 * Third + 10);
									newButton.BackColor = Color.LightGray;
									newButton.TextAlign = ContentAlignment.MiddleCenter;
									var propertyInfo = checkitems.GetType().GetProperty("Value");
									var TextPackage = propertyInfo.GetValue(checkitems, null);
									newButton.Tag = TextPackage;
									newButton.Text = checkitems.ToString();
									panelthirdparty.Controls.Add(newButton);
									newButton.Click += new EventHandler(this.DynamicButton_Click);
									Third++;
								}
								ThirdPartyLogin.Clear();
							}
							else
							{
								MessageBox.Show("Invalid! Your Approved Member Count is " + StartBooking.Noofmembers);
							}
						}
						else
						{
							SubmitPackage();
							foreach (var checkitems in ThirdPartyLogin)
							{
								Button newButton = new Button();
								newButton.Location = new Point(20, 30 * Third + 10);
								newButton.BackColor = Color.LightGray;
								newButton.TextAlign = ContentAlignment.MiddleCenter;
								var propertyInfo = checkitems.GetType().GetProperty("Value");
								var TextPackage = propertyInfo.GetValue(checkitems, null);
								newButton.Tag = TextPackage;
								newButton.Text = checkitems.ToString();
								panelthirdparty.Controls.Add(newButton);
								newButton.Click += new EventHandler(this.DynamicButton_Click);
								Third++;
							}
							ThirdPartyLogin.Clear();
						}
					}
					else
					{
						if (StartBooking.StaffFk != null || StartBooking.Schoolfk != null)
						{
							if (StartBooking.Noofmembers <= Totalmembercalculate)
							{
								SubmitTickets();
								foreach (var checkitems in ThirdPartyLogin)
								{
									Button newButton = new Button();
									newButton.Location = new Point(20, 30 * Third + 10);
									newButton.BackColor = Color.LightGray;
									newButton.TextAlign = ContentAlignment.MiddleCenter;
									var propertyInfo = checkitems.GetType().GetProperty("Value");
									var TextPackage = propertyInfo.GetValue(checkitems, null);
									var propertyInfo1 = checkitems.GetType().GetProperty("tagvalue");
									var TextPackage1 = propertyInfo1.GetValue(checkitems, null);
									newButton.Tag = TextPackage;
									newButton.Text = TextPackage1.ToString();
									panelthirdparty.Controls.Add(newButton);
									newButton.Click += new EventHandler(this.DynamicButton_Click);
									Third++;
								}
								ThirdPartyLogin.Clear();
							}
							else
							{
								MessageBox.Show("Invalid! Your Approved Member Count is " + StartBooking.Noofmembers);
							}
						}
						else
						{
							SubmitTickets();
							foreach (var checkitems in ThirdPartyLogin)
							{
								Button newButton = new Button();
								newButton.Location = new Point(20, 30 * Third + 10);
								newButton.BackColor = Color.LightGray;
								newButton.TextAlign = ContentAlignment.MiddleCenter;
								var propertyInfo = checkitems.GetType().GetProperty("Value");
								var TextPackage = propertyInfo.GetValue(checkitems, null);
								var propertyInfo1 = checkitems.GetType().GetProperty("tagvalue");
								var TextPackage1 = propertyInfo1.GetValue(checkitems, null);
								newButton.Tag = TextPackage;
								newButton.Text = TextPackage1.ToString();
								panelthirdparty.Controls.Add(newButton);
								newButton.Click += new EventHandler(this.DynamicButton_Click);
								Third++;
							}
							ThirdPartyLogin.Clear();
						}
					}
					if (StartBooking.Noofmembers >= Totalmembercalculate)
					{
						if (StartBooking.Schoolfk != null)
						{
							SubmitTicketSchool();
						}
					}
				}
			}
			else if (txt_MobileNumber.Text.ToString().Trim() == "")
			{
				errorProvider1.SetError(txt_MobileNumber, "Please enter the MobileNumber");
			}
			else if (txt_netAmount.Text.ToString().Trim() == "")
			{
				errorProvider1.SetError(txt_netAmount, "Please enter the NoofMembers");
			}
			else if (Cmb_PaymentMode.Text.ToString().Trim() == "")
			{
				errorProvider1.SetError(Cmb_PaymentMode, "Please enter the Payment Mode");
			}
			else if (txt_visitdate.Text.ToString().Trim() == "")
			{
				errorProvider1.SetError(txt_visitdate, "Please Select the Visit Date");
			}
		}

		private void DynamicButton_Click(object sender, EventArgs e)
		{
			pakname = null;
			string s = (sender as Button).Tag.ToString();
			Tickets.Checkthirdparty = Printcheck.Thirdparty.ToString();
			ThirdpartyId = s;
			TicketData tkt = new TicketData();
			tkt.Show();
		}

		private void dgridCategory_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Tab)
			{
				bool isChecked = rbd_Package.Checked;
				if (isChecked == true)
				{
					PackageCalculation();
				}
				else
				{
					TicketsGridCalculation();
				}
			}
		}

		private void Cmb_PaymentMode_SelectedIndexChanged(object sender, EventArgs e)
		{
			string prc = txt_netAmount.Text.Replace("₹", "").Replace("/-", "");
			if (!string.IsNullOrEmpty(prc))
			{
				if (Cmb_PaymentMode.Text.ToString() == PaymentMode.CARD.ToString())
				{
					//miscellaneous value = miscellaneous.Tax;
					//int val = (int)value;
					txt_mislaneous.Visible = true;
					txt_mislaneous.Text = MyTax + "%";
					txt_mislaneous.ReadOnly = true;
					string mislanous = txt_mislaneous.Text.Replace("%", "");
					decimal NewPrice = Convert.ToDecimal((decimal.Parse(prc) * decimal.Parse(mislanous)) / 100);
					decimal prc1 = Convert.ToDecimal(NewPrice + Convert.ToDecimal(prc));
					txt_netAmount.Text = "₹ " + string.Format("{0:0.00}", prc1) + " /-";
					txtTT.Text = string.Format("{0:0.00}", Convert.ToDecimal(prc.Trim()));
					MislaneousCharges = string.Format("{0:0.00}", NewPrice);
				}
				else
				{
					txt_mislaneous.Visible = false;
					txt_netAmount.Text = "₹ " + FinalTotalCash.ToString() + " /-";
					txtTT.Text = txt_netAmount.Text;
					MislaneousCharges = "0";
				}
			}
		}

		private void lst_Package_ItemCheck_1(object sender, ItemCheckEventArgs e)
		{
			bool isChecked = rbd_Package.Checked;
			if (isChecked == true)
			{
				for (int ix = 0; ix < lst_Package.Items.Count; ++ix)
					if (ix != e.Index) lst_Package.SetItemChecked(ix, false);
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			bool isChecked = rbd_Package.Checked;
			if (isChecked == true)
			{
				if (lst_Package.SelectedIndex > -1)
				{
					progressBar1.Visible = true;
					progressBar1.Style = ProgressBarStyle.Marquee;
					Thread thread =
					new Thread(new ThreadStart(PackageGrid));
					thread.Start();
					checkthirdparty();
					but_prints.Visible = false;
					but_mislaneous.Visible = false;
					panelthirdparty.Controls.Clear();
				}
				else
				{
					MessageBox.Show("Please select atleast one Item.");
				}
			}
			else
			{
				but_prints.Visible = false;
				but_mislaneous.Visible = false;
				panelthirdparty.Controls.Clear();
				progressBar1.Visible = true;
				progressBar1.Style = ProgressBarStyle.Marquee;
				Thread thread =
				new Thread(new ThreadStart(TicketsGrid));
				thread.Start();
			}
		}

		private void rbd_tickets_Click(object sender, EventArgs e)
		{
			but_prints.Visible = false;
			but_mislaneous.Visible = false;
			panelthirdparty.Controls.Clear();
			lst_Package.Visible = true;
			lbl_package.Text = "Tickets";
			lbl_package.Visible = true;
			GetTickets();
			valueFalse();
			txt_mislaneous.Visible = false;
		}

		private void rbd_Package_Click(object sender, EventArgs e)
		{
			but_prints.Visible = false;
			but_mislaneous.Visible = false;
			panelthirdparty.Controls.Clear();
			lst_Package.Visible = true;
			lbl_package.Visible = true;
			lbl_package.Text = "Packages";
			GetPAckage();
			valueFalse();
			txt_mislaneous.Visible = false;
			lst_Package.SelectionMode = SelectionMode.One;
		}

		private void txt_MobileNumber_Validating_1(object sender, CancelEventArgs e)
		{
			if (ValidatePhone(txt_MobileNumber.Text))
			{
				errorProvider2.Clear();
			}
			else
			{
				errorProvider2.SetError(txt_MobileNumber, "Invalid phone number");
				this.GiveFocusToControlIfTabPage(this.Parent);
			}
		}

		private void txtemail_Validating(object sender, CancelEventArgs e)
		{

		}

		private void GiveFocusToControlIfTabPage(Control ctrl)
		{
			if (ctrl == null)
			{
				return;
			}
			if (ctrl is TabPage)
			{
				TabPage tabPage = (TabPage)ctrl;
				((TabControl)tabPage.Parent).SelectedTab = tabPage;
				return;
			}
			this.GiveFocusToControlIfTabPage(ctrl.Parent);
		}

		private void dgridCategory_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
		{
			int colCount = dgridCategory.Rows[0].Cells.Count - 3;
			bool isChecked = rbd_Package.Checked;
			if (isChecked == true)
			{
				if (e.ColumnIndex == 5) // 1 should be your column index
				{
					int i;
					if (!int.TryParse(Convert.ToString(e.FormattedValue), out i))
					{
						e.Cancel = true;
						MessageBox.Show("Please enter numeric");
					}
					else
					{
						// the input is numeric 
					}
				}
			}
			else
			{
				if (e.ColumnIndex == colCount) // 1 should be your column index
				{
					int i;
					if (!int.TryParse(Convert.ToString(e.FormattedValue), out i))
					{
						e.Cancel = true;
						MessageBox.Show("Please enter numeric");
					}
					else
					{
						// the input is numeric 
					}
				}
			}
		}

		private void dgridCategory_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
		{
			this.dgridCategory.ScrollBars = ScrollBars.Both;
		}

		private void Tickets_Load(object sender, EventArgs e)
		{
			groupBox1.Hide();
		}

		private void txt_visitdate_Enter(object sender, EventArgs e)
		{
			monthCalendar1.Visible = true;
		}

		private void txt_visitdate_Leave(object sender, EventArgs e)
		{
			if (!monthCalendar1.Focused)
				monthCalendar1.Visible = false;
		}

		private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
		{
			var monthCalendar = sender as MonthCalendar;
			txt_visitdate.Text = monthCalendar.SelectionStart.ToString();
			monthCalendar1.Visible = false;
		}

		private void monthCalendar1_Leave(object sender, EventArgs e)
		{
			var monthCalendar = sender as MonthCalendar;
			monthCalendar.Visible = false;
		}

		private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
		{
			txt_visitdate.Text = monthCalendar1.SelectionRange.Start.ToString();
		}

		private void but_search_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(txt_search.Text))
			{
				string data = Common.GetData(string.Format("{0}/api/Ticket/SearchPhoneNo?MobileNunber={1}", appsetting, txt_search.Text));
				var jsonObject = JsonConvert.DeserializeObject<RootObject_Memberdetails>(data);
				if (jsonObject != null)
				{
					txt_FirstName.Text = jsonObject.FirstName;
					txt_LastName.Text = jsonObject.LastName;
					txtemail.Text = jsonObject.Email;
					txt_MobileNumber.Text = jsonObject.MobileNumber;
				}
				else
				{
					MessageBox.Show("Please enter valid Phone No");
					txt_search.Text = string.Empty;
				}
			}
			else
			{
				MessageBox.Show("Please enter MobileNumber");
			}
		}

		private void but_prints_Click_1(object sender, EventArgs e)
		{
			Checkthirdparty = null;
			bool isChecked = rbd_Package.Checked;
			if (isChecked == true)
			{
				Tickets.pakname = Printcheck.Packages.ToString();
			}
			else
			{
				Tickets.pakname = Printcheck.Prints.ToString();
			}
			Print tdata = new Print();
			tdata.Show();
		}

		private void but_mislaneous_Click_1(object sender, EventArgs e)
		{
			Checkthirdparty = null;
			Tickets.pakname = Printcheck.Mislaneous.ToString();
			TicketData tkt = new TicketData();
			tkt.Show();
		}

		private void button2_Click_1(object sender, EventArgs e)
		{
			Shows shows = new Shows();
			shows.ShowDialog();
		}

		#endregion

	}
}
