using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace raBudget.EfPersistence.Migrations
{
    public partial class _20190921 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    CurrencyCode = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: false),
                    Symbol = table.Column<string>(nullable: false),
                    EnglishName = table.Column<string>(nullable: false),
                    NativeName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.CurrencyCode);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    DefaultBudgetId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Budget",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    CurrencyCode = table.Column<int>(nullable: false),
                    StartingDate = table.Column<DateTime>(nullable: false),
                    OwnedByUserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budget", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Budget_Users_OwnedByUserId",
                        column: x => x.OwnedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BudgetCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<int>(nullable: false),
                    Icon = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    BudgetId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetCategories_Budget_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BudgetShares",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BudgetId = table.Column<int>(nullable: false),
                    SharedWithUserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetShares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetShares_Budget_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetShares_Users_SharedWithUserId",
                        column: x => x.SharedWithUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Allocations",
                columns: table => new
                {
                    AllocationId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    AllocationDateTime = table.Column<DateTime>(nullable: false),
                    CreationDateTime = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(2019, 9, 21, 20, 7, 15, 678, DateTimeKind.Local).AddTicks(2340)),
                    CreatedByUserId = table.Column<Guid>(nullable: false),
                    BudgetCategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Allocations", x => x.AllocationId);
                    table.ForeignKey(
                        name: "FK_Allocations_BudgetCategories_BudgetCategoryId",
                        column: x => x.BudgetCategoryId,
                        principalTable: "BudgetCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Allocations_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BudgetCategoryBudgetedAmounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MonthlyAmount = table.Column<double>(nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: false),
                    ValidTo = table.Column<DateTime>(nullable: true),
                    BudgetCategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetCategoryBudgetedAmounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetCategoryBudgetedAmounts_BudgetCategories_BudgetCategor~",
                        column: x => x.BudgetCategoryId,
                        principalTable: "BudgetCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransactionSchedules",
                columns: table => new
                {
                    TransactionScheduleId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BudgetCategoryId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    Frequency = table.Column<int>(nullable: false),
                    PeriodStep = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionSchedules", x => x.TransactionScheduleId);
                    table.ForeignKey(
                        name: "FK_TransactionSchedules_BudgetCategories_BudgetCategoryId",
                        column: x => x.BudgetCategoryId,
                        principalTable: "BudgetCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    TransactionDateTime = table.Column<DateTime>(nullable: false),
                    CreationDateTime = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(2019, 9, 21, 20, 7, 15, 822, DateTimeKind.Local).AddTicks(7742)),
                    CreatedByUserId = table.Column<Guid>(nullable: false),
                    BudgetCategoryId = table.Column<int>(nullable: false),
                    TransactionScheduleId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_BudgetCategories_BudgetCategoryId",
                        column: x => x.BudgetCategoryId,
                        principalTable: "BudgetCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_TransactionSchedules_TransactionScheduleId",
                        column: x => x.TransactionScheduleId,
                        principalTable: "TransactionSchedules",
                        principalColumn: "TransactionScheduleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "CurrencyCode", "Code", "EnglishName", "NativeName", "Symbol" },
                values: new object[,]
                {
                    { 8, "ALL", "Albanian Lek", "Leku shqiptar", "Lekë" },
                    { 760, "SYP", "Syrian Pound", "ليرة سوري", "ل.س.‏" },
                    { 756, "CHF", "Swiss Franc", "Schweizer Franken", "CHF" },
                    { 752, "SEK", "Swedish Krona", "ruvdnu", "kr" },
                    { 710, "ZAR", "South African Rand", "Suid-Afrikaanse rand", "R" },
                    { 704, "VND", "Vietnamese Dong", "Đồng", "₫" },
                    { 702, "SGD", "Singapore Dollar", "Singapore Dollar", "$" },
                    { 682, "SAR", "Saudi Riyal", "ريال سعودي", "ر.س.‏" },
                    { 646, "RWF", "Rwandan Franc", "Rwandan Franc", "RF" },
                    { 643, "RUB", "Russian Ruble", "һум", "₽" },
                    { 764, "THB", "Thai Baht", "บาท", "฿" },
                    { 634, "QAR", "Qatari Rial", "ريال قطري", "ر.ق.‏" },
                    { 604, "PEN", "Peruvian Sol", "sol peruano", "S/" },
                    { 600, "PYG", "Paraguayan Guarani", "guaraní paraguayo", "₲" },
                    { 590, "PAB", "Panamanian Balboa", "balboa panameño", "B/." },
                    { 586, "PKR", "Pakistani Rupee", "روپيه", "Rs" },
                    { 578, "NOK", "Norwegian Krone", "norske kroner", "kr" },
                    { 558, "NIO", "Nicaraguan Córdoba", "córdoba nicaragüense", "C$" },
                    { 554, "NZD", "New Zealand Dollar", "New Zealand Dollar", "$" },
                    { 524, "NPR", "Nepalese Rupee", "रुपैयाँ", "रु" },
                    { 512, "OMR", "Omani Rial", "ريال عماني", "ر.ع.‏" },
                    { 608, "PHP", "Philippine Piso", "Philippine Piso", "₱" },
                    { 504, "MAD", "Moroccan Dirham", "درهم مغربي", "د.م.‏" },
                    { 780, "TTD", "Trinidad and Tobago Dollar", "Trinidad and Tobago Dollar", "$" },
                    { 788, "TND", "Tunisian Dinar", "دينار تونسي", "د.ت.‏" },
                    { 981, "GEL", "Georgian Lari", "ქართული ლარი", "₾" },
                    { 980, "UAH", "Ukrainian Hryvnia", "гривня", "₴" },
                    { 978, "EUR", "Euro", "euro", "€" },
                    { 977, "BAM", "Bosnia-Herzegovina Convertible Mark", "конвертибилна марка", "КМ" },
                    { 975, "BGN", "Bulgarian Lev", "български лев", "лв." },
                    { 972, "TJS", "Tajikistani Somoni", "Сомонӣ", "смн" },
                    { 971, "AFN", "Afghan Afghani", "افغانى", "؋" },
                    { 952, "XOF", "West African CFA Franc", "CFA", "CFA" },
                    { 949, "TRY", "Turkish Lira", "Türk Lirası", "₺" },
                    { 784, "AED", "United Arab Emirates Dirham", "درهم اماراتي", "د.إ.‏" },
                    { 946, "RON", "Romanian Leu", "leu românesc", "lei" },
                    { 941, "RSD", "Serbian Dinar", "динар", "дин." },
                    { 901, "TWD", "New Taiwan Dollar", "新台幣", "NT$" },
                    { 886, "YER", "Yemeni Rial", "ريال يمني", "ر.ي.‏" },
                    { 860, "UZS", "Uzbekistani Som", "Ўзбекистон сўм", "сўм" },
                    { 858, "UYU", "Uruguayan Peso", "peso uruguayo", "$" },
                    { 840, "USD", "US Dollar", "US Dollar", "$" },
                    { 826, "GBP", "British Pound", "Punt Prydain", "£" },
                    { 818, "EGP", "Egyptian Pound", "جنيه مصري", "ج.م.‏" },
                    { 807, "MKD", "Macedonian Denar", "Македонски денар", "ден" },
                    { 944, "AZN", "Azerbaijani Manat", "Aзәрбајҹан манаты", "₼" },
                    { 985, "PLN", "Polish Zloty", "złoty polski", "zł" },
                    { 496, "MNT", "Mongolian Tugrik", "төгрөг", "₮" },
                    { 462, "MVR", "Maldivian Rufiyaa", "ރުފިޔާ", "ރ." },
                    { 214, "DOP", "Dominican Peso", "peso dominicano", "$" },
                    { 208, "DKK", "Danish Krone", "Dansk krone", "kr." },
                    { 203, "CZK", "Czech Koruna", "česká koruna", "Kč" },
                    { 191, "HRK", "Croatian Kuna", "hrvatska kuna", "kn" },
                    { 188, "CRC", "Costa Rican Colón", "colón costarricense", "₡" },
                    { 170, "COP", "Colombian Peso", "peso colombiano", "$" },
                    { 156, "CNY", "Chinese Yuan", "མི་དམངས་ཤོག་སྒོཪ།", "¥" },
                    { 152, "CLP", "Chilean Peso", "Peso", "$" },
                    { 144, "LKR", "Sri Lankan Rupee", "ශ්‍රී ලංකා රුපියල", "රු." },
                    { 230, "ETB", "Ethiopian Birr", "የኢትዮጵያ ብር", "ብር" },
                    { 124, "CAD", "Canadian Dollar", "Canadian Dollar", "$" },
                    { 96, "BND", "Brunei Dollar", "Dolar Brunei", "$" },
                    { 84, "BZD", "Belize Dollar", "Belize Dollar", "$" },
                    { 68, "BOB", "Bolivian Boliviano", "boliviano", "Bs" },
                    { 51, "AMD", "Armenian Dram", "դրամ", "֏" },
                    { 50, "BDT", "Bangladeshi Taka", "বাংলাদেশী টাকা", "৳" },
                    { 48, "BHD", "Bahraini Dinar", "دينار بحريني", "د.ب.‏" },
                    { 36, "AUD", "Australian Dollar", "Australian Dollar", "$" },
                    { 32, "ARS", "Argentine Peso", "peso argentino", "$" },
                    { 12, "DZD", "Algerian Dinar", "دينار جزائري", "د.ج.‏" },
                    { 116, "KHR", "Cambodian Riel", "x179Aៀល", "៛" },
                    { 484, "MXN", "Mexican Peso", "Peso", "$" },
                    { 320, "GTQ", "Guatemalan Quetzal", "quetzal", "Q" },
                    { 344, "HKD", "Hong Kong Dollar", "Hong Kong Dollar", "$" },
                    { 458, "MYR", "Malaysian Ringgit", "Ringgit Malaysia", "RM" },
                    { 446, "MOP", "Macanese Pataca", "澳門幣", "MOP" },
                    { 434, "LYD", "Libyan Dinar", "دينار ليبي", "د.ل.‏" },
                    { 422, "LBP", "Lebanese Pound", "ليرة لبناني", "ل.ل.‏" },
                    { 418, "LAK", "Laotian Kip", "ລາວ ກີບ", "₭" },
                    { 417, "KGS", "Kyrgystani Som", "сом", "сом" },
                    { 414, "KWD", "Kuwaiti Dinar", "دينار كويتي", "د.ك.‏" },
                    { 410, "KRW", "South Korean Won", "원", "₩" },
                    { 404, "KES", "Kenyan Shilling", "Shilingi ya Kenya", "Ksh" },
                    { 340, "HNL", "Honduran Lempira", "lempira hondureño", "L" },
                    { 400, "JOD", "Jordanian Dinar", "دينار اردني", "د.ا.‏" },
                    { 392, "JPY", "Japanese Yen", "円", "¥" },
                    { 388, "JMD", "Jamaican Dollar", "Jamaican Dollar", "$" },
                    { 376, "ILS", "Israeli New Shekel", "שקל חדש", "₪" },
                    { 368, "IQD", "Iraqi Dinar", "دينار عراقي", "د.ع.‏" },
                    { 364, "IRR", "Iranian Rial", "ریال", "ريال" },
                    { 360, "IDR", "Indonesian Rupiah", "Rupiah", "Rp" },
                    { 356, "INR", "Indian Rupee", "টকা", "₹" },
                    { 352, "ISK", "Icelandic Króna", "íslensk króna", "kr" },
                    { 348, "HUF", "Hungarian Forint", "magyar forint", "Ft" },
                    { 398, "KZT", "Kazakhstani Tenge", "Қазақстан теңгесі", "₸" },
                    { 986, "BRL", "Brazilian Real", "Real", "R$" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Allocations_AllocationDateTime",
                table: "Allocations",
                column: "AllocationDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_Allocations_BudgetCategoryId",
                table: "Allocations",
                column: "BudgetCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Allocations_CreatedByUserId",
                table: "Allocations",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Budget_OwnedByUserId",
                table: "Budget",
                column: "OwnedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetCategories_BudgetId",
                table: "BudgetCategories",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetCategoryBudgetedAmounts_BudgetCategoryId",
                table: "BudgetCategoryBudgetedAmounts",
                column: "BudgetCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetShares_BudgetId",
                table: "BudgetShares",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetShares_SharedWithUserId",
                table: "BudgetShares",
                column: "SharedWithUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Currency_Code",
                table: "Currency",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_BudgetCategoryId",
                table: "Transactions",
                column: "BudgetCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CreatedByUserId",
                table: "Transactions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionDateTime",
                table: "Transactions",
                column: "TransactionDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionScheduleId",
                table: "Transactions",
                column: "TransactionScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionSchedules_BudgetCategoryId",
                table: "TransactionSchedules",
                column: "BudgetCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionSchedules_StartDate",
                table: "TransactionSchedules",
                column: "StartDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Allocations");

            migrationBuilder.DropTable(
                name: "BudgetCategoryBudgetedAmounts");

            migrationBuilder.DropTable(
                name: "BudgetShares");

            migrationBuilder.DropTable(
                name: "Currency");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "TransactionSchedules");

            migrationBuilder.DropTable(
                name: "BudgetCategories");

            migrationBuilder.DropTable(
                name: "Budget");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
