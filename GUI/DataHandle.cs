using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace DataHandle
{
    public class DataHandle
    {
        string except_flag ="";
        private static DataHandle instance;
        public static DataHandle Instance
        {
            get { if (instance == null) instance = new DataHandle(); return instance; }
            private set { instance = value; }
        }
        private DataHandle() { }
        //hàm xử lý generate code tổng
        public List<string> All_Handle(ref string flag_exception, string class_name, string input)
        {
            var list_lines_final = new List<string>();
            input = input.Replace(" ", String.Empty);
            string[] lines = input.Split(new[] {"pre", "post" }, StringSplitOptions.None);
            string[] first_line = lines[0].Split(new[] { "(", ")" }, StringSplitOptions.None);
            //ten ham
            try
            {
                string name = first_line[0];
                //lay parameter dau vao cua ham
                string parameter = first_line[1];
                //khoi tao danh sach bien
                var list_var = new List<string>();
                GetListValue(ref list_var, lines[0]);
                list_lines_final.Add("using| System;");
                list_lines_final.Add("namespace| FormalSpecification");
                list_lines_final.Add("{");//name space
                list_lines_final.Add(string.Format("\tpublic| class| {0}", class_name));
                list_lines_final.Add("\t{");//class program
                TitleHandle(ref list_lines_final, lines[0]);
                PreHandle(ref list_lines_final, name, parameter, ExceptionPreHandle(lines[1]));
                PostHandle(ref list_lines_final, name, parameter, ExceptionPostHandle(lines[2]), list_var);
                MainHandle(ref list_lines_final, name, parameter, list_var, class_name);
                list_lines_final.Add("\t}");//class program
                list_lines_final.Add("}");//name space
                flag_exception = except_flag;
                return list_lines_final;
            }
            catch
            {
                flag_exception = "Invalid Input";
                return list_lines_final;
            }
            
        }
        //hàm xử lý dòng đầu của input
        public void TitleHandle(ref List<string> list, string input)
        {
            try
            {
                string[] lines = input.Split(new[] {"(", ")"}, StringSplitOptions.None);
                InputVarHandle(ref list, lines[0], lines[1]);
                OutputVarHandle(ref list, lines[0], lines[2]);
            }
            catch
            {
                except_flag = "Invalid Title";
                Console.WriteLine("Invalid Title");
                return;
            }
        }
        //hàm xử lý dòng pre
        public void PreHandle(ref List<string> list, string name, string parameter, string pre)
        { 
            try
            {
                //xu ly dau ngoac du hoac thieu
                pre = pre.Replace(" ", String.Empty);
                pre = pre.Replace("(", String.Empty);
                pre = pre.Replace(")", String.Empty);
                string[] temp_pre = pre.Split(new[] { "&&" }, StringSplitOptions.None);
                if (temp_pre.Length > 1)
                {
                    //sửa lại pre cho đúng theo dấu ngoặc
                    pre = "";
                    for (int i = 0; i < temp_pre.Length; i++)
                    {
                        if (i == 0)
                            pre += string.Format("({0})", temp_pre[0]);
                        else
                            pre += string.Format(" && ({0})", temp_pre[0]);
                    }
                }
                //
                try
                {

                    string name_function = string.Format("\t\tpublic| int| KiemTra_{0}", GetParameter(name, parameter)[0]);
                    list.Add(name_function);
                    list.Add("\t\t{");//ngoac ham kiem tra
                    if (pre == "")
                    {
                        list.Add("\t\t\treturn| 1;");
                    }
                    else
                    {
                        string condition = string.Format("\t\t\tif| ({0})", pre);
                        list.Add(condition);
                        list.Add("\t\t\t\treturn| 1;");
                        list.Add("\t\t\telse");
                        list.Add("\t\t\t\treturn| 0;");
                    }
                    list.Add("\t\t}");//ngoac ham kiem tra}
                }
                catch
                {
                    except_flag = "Invalid Pre";
                    Console.WriteLine("Loi PreHandle");
                }
            }
            catch
            {
                except_flag = "Invalid Pre";
                Console.WriteLine("Invalid Pre");
            }
        }
        //xử lý biểu thức trong post
        public List<string> ExpressionHandle(string expression, string name_result)
        {
            int temp = 0; //lưu vị trí của biểu thức chứa dấu =
            expression = expression.Replace(" ", String.Empty);
            expression = expression.Replace(")", String.Empty);
            expression = expression.Replace("(", String.Empty);
            Console.WriteLine(expression);
            var expression_lines = new List<string>();
            try
            {
                
                string[] split_expression = expression.Split(new[] { "&&" }, StringSplitOptions.None);
                string condition_line = "";
                //nếu biểu thức chỉ có 1 điều kiện thì 1 cặp ngoặc ngoài
                if (split_expression.Length == 1)
                {
                    if (split_expression[0].Contains("=") &&
                        split_expression[0].Contains(name_result))
                    {
                        expression_lines.Add(string.Format("{0};", split_expression[temp]));
                    }
                    else
                    {
                        Console.WriteLine("Loi tai ham tinh toan bieu thuc post");
                    }
                }
                else if (split_expression.Length == 2)
                {
                    if (split_expression[0].Contains("=") &&
                        split_expression[0].Contains(name_result))
                    {
                        temp = 0;
                        if (split_expression[1].Contains("=") &&
                            !split_expression[1].Contains(">=") &&
                            !split_expression[1].Contains("<=") &&
                            !split_expression[1].Contains("!=") &&
                            !split_expression[1].Contains("=="))
                        {
                            condition_line = string.Format("if| ({0})", split_expression[1]).Replace("=", "==");
                        }
                        else
                            condition_line = string.Format("if| ({0})", split_expression[1]);

                    }
                    else if (split_expression[1].Contains("=") &&
                        split_expression[1].Contains(name_result))
                    {
                        temp = 1;
                        if (split_expression[0].Contains("=") &&
                            !split_expression[0].Contains(">=") &&
                            !split_expression[0].Contains("<=") &&
                            !split_expression[0].Contains("!=") &&
                            !split_expression[0].Contains("=="))
                        {
                            condition_line = string.Format("if| ({0})", split_expression[0]).Replace("=", "==");
                        }
                        else
                            condition_line = string.Format("if| ({0})", split_expression[0]);
                    }
                    else
                    {
                        Console.WriteLine("Loi o phan expression");
                    }
                    expression_lines.Add(condition_line);
                    expression_lines.Add(string.Format("\t{0};", split_expression[temp]));
                }
                //nếu biểu thức có hơn 1 điều kiện
                else
                {
                    //dùng để kiểm tra xem biểu thức đầu tiên có phải là biểu thức có chứa dấu = không
                    int flag = 0;
                    condition_line = "if| (";
                    for (int i = 0; i < split_expression.Length; i++)
                    {
                        //nếu biểu thức chứa biến kết quả và chứa dấu = ( k phải >= , <=, !=, ==) thì biểu thức đó là biểu thức cần gán
                        if (split_expression[i].Contains(name_result) &&
                            split_expression[i].Contains("=") &&
                            !split_expression[i].Contains(">=") &&
                            !split_expression[i].Contains("<=") &&
                            !split_expression[i].Contains("!=") &&
                            !split_expression[i].Contains("=="))
                        {
                            temp = i;
                            flag = 1;
                        }
                        else
                        {
                            // trong trường hợp biểu thức điều kiện chỉ có 1 dấu = thì thay bằng == theo kiểu so sánh c#
                            if (split_expression[i].Contains("=") &&
                            !split_expression[i].Contains(">=") &&
                            !split_expression[i].Contains("<=") &&
                            !split_expression[i].Contains("!=") &&
                            !split_expression[i].Contains("=="))
                            {
                                split_expression[i] = split_expression[i].Replace("=", "==");
                            }
                            //nếu là điều kiện đầu tiên thì không cần && ở trước
                            if ((i == 0 && flag == 0) || (i == 1))
                                condition_line += string.Format("({0})", split_expression[i]);
                            else
                                condition_line += string.Format(" && ({0})", split_expression[i]);
                        }
                    }
                    condition_line += ")";
                    expression_lines.Add(condition_line);
                    expression_lines.Add(string.Format("\t{0};", split_expression[temp]));
                }
                return expression_lines;
            }
            catch
            {
                except_flag = "Invalid Expression";
                Console.WriteLine("Invalid Expression");
                return expression_lines;
            }
        }
        //xử lý hàm post để generate code giải thuật toán
        public void PostHandle(ref List<string> list, string name, string parameter, string post, List<string> list_var)
        {
            try
            {
                string name_function = string.Format("\t\tpublic| {0} |XuLy_{1}", list_var[1], GetParameter(name, parameter)[0]);
                list.Add(name_function);
                list.Add("\t\t{"); //ngoac ham xu ly
                                   //nếu kiểu dữ liệu là string thì gán = "", còn k thì gán bằng 0
                if (list_var[1] == "|string|")
                    list.Add(string.Format("\t\t\t{0} {1} = \"\";", list_var[1], list_var[0]));
                else if (list_var[1] == "|bool|")
                    list.Add(string.Format("\t\t\t{0} {1} = false;", list_var[1], list_var[0]));
                else
                {
                    list.Add(string.Format("\t\t\t{0} {1} = 0;", list_var[1], list_var[0]));
                }
                //tách biểu thức lớn thành nhiều biểu thức nhỏ
                string[] expressions = post.Split(new[] { "||" }, StringSplitOptions.None);
                //chạy từng biểu thức nhỏ
                for (int i = 0; i < expressions.Length; i++)
                {
                    //lấy danh sách các dòng của biểu thức nhỏ, chạy vòng lặp để thêm từng dòng vào list final
                    var expression_lines = ExpressionHandle(expressions[i], list_var[0]);
                    for (int j = 0; j < expression_lines.Count; j++)
                    {
                        string line = string.Format("\t\t\t{0}", expression_lines[j]).Replace("FALSE", "false");
                        list.Add(line.Replace("TRUE","true"));
                    }
                }
                list.Add(string.Format("\t\t\treturn| {0};", list_var[0]));
                list.Add("\t\t}");//ngoac ham xu ly
            }
            catch
            {
                except_flag = "Invalid Post";
                Console.WriteLine("Loi PostHandle");
            }

        }
        //generate hàm main
        public void MainHandle(ref List<string> list, string name, string parameter, List<string> list_var, string class_name)
        {
            try
            {
                list.Add("\t\tpublic| static| void| Main|(|string[]| args)");
                list.Add("\t\t{"); //ngoac ham main
                for (int i = 0; i < list_var.Count; i += 2)
                {
                    if (list_var[i+1] == "|string|")
                        list.Add(string.Format("\t\t\t{0} {1} = \"\";", list_var[i+1], list_var[i]));
                    else if (list_var[i+1] == "|bool|")
                        list.Add(string.Format("\t\t\t{0} {1} = false;", list_var[i+1], list_var[i]));
                    else
                    {
                        list.Add(string.Format("\t\t\t{0} {1} = 0;", list_var[i+1], list_var[i]));
                    };
                }    
                list.Add(string.Format("\t\t\t{0}| p| = |new |{0}|();", class_name));
                list.Add(string.Format("\t\t\tp|.|Nhap_{0}|;", GetParameter(name, parameter)[1]));
                list.Add(string.Format("\t\t\tif |(|p|.|KiemTra_{0}| == 1)", GetParameter(name, parameter)[2]));
                list.Add("\t\t\t{"); //ngoac ham if condition
                list.Add(string.Format("\t\t\t\t{0} = |p|.|XuLy_{1}|;", list_var[0], GetParameter(name, parameter)[2]));
                list.Add(string.Format("\t\t\t\t|p|.Xuat_{0}|({1});", name, list_var[0]));
                list.Add("\t\t\t}");//ngoac ham if condition
                list.Add("\t\t\telse");
                list.Add("\t\t\t\tConsole|.WriteLine|(|\"Thong tin nhap khong hop le!\"|);");
                list.Add("\t\t\tConsole|.ReadLine|();");
                list.Add("\t\t}"); //ngoac ham main
            }
            catch
            {
                except_flag = "Invalid Main";
                Console.WriteLine("Loi MainHandle");
            }
        }
        //tạo ra các biểu thức (.... )để tái sử dụng nhiều lần
        public List<string> GetParameter(string name, string input)
        {
            var list_parameter = new List<string>();
            string[] input_chars = input.Split(new[] { ":", "," }, StringSplitOptions.None);
            if (input_chars.Length % 2 != 0 || input_chars.Length < 2)
            {
                except_flag = "Invalid Parameter";
                Console.WriteLine("so luong bien input khong dung");
                return list_parameter;
            }
            else
            {
                try
                {
                    //ten ham
                    //[0] là tên hàm(float a, float b)
                    string name_function = string.Format("{0}|(", name);
                    for (int i = 0; i < input_chars.Length; i += 2)
                    {
                        if (i != 0)
                            name_function += ", ";
                        string type = GetTypeData(input_chars[i + 1]);
                        string var = string.Format("{0} {1}", type, input_chars[i]);
                        name_function += var;
                    }
                    name_function += ")";
                    list_parameter.Add(name_function);

                    //[1] là tên hàm(ref a, ref b)
                    name_function = string.Format("{0}|(", name);
                    for (int i = 0; i < input_chars.Length; i += 2)
                    {
                        if (i != 0)
                            name_function += ", ";
                        string var = string.Format("|ref| {0}", input_chars[i]);
                        name_function += var;
                    }
                    name_function += ")";
                    list_parameter.Add(name_function);

                    //[2] là tên hàm(a, b)
                    name_function = string.Format("{0}|(", name);
                    for (int i = 0; i < input_chars.Length; i += 2)
                    {
                        if (i != 0)
                            name_function += ", ";
                        string var = string.Format("{0}", input_chars[i]);
                        name_function += var;
                    }
                    name_function += ")";
                    list_parameter.Add(name_function);
                    return list_parameter;
                }
                catch
                {
                    except_flag = "Invalid Parameter";
                    Console.WriteLine("Lỗi InputVarHandle");
                    return list_parameter;
                }
            }
        }
        //tạo 1 list các biến và kiểu dữ liệu biến
        public void GetListValue(ref List<string> list_var, string input)
        {
            try
            {
                string[] lines = input.Split(new[] { "(", ")" }, StringSplitOptions.None);
                string[] output_chars = lines[2].Split(new[] { ":", "," }, StringSplitOptions.None);
                string[] input_chars = lines[1].Split(new[] { ":", "," }, StringSplitOptions.None);
                //list_var[0] = ten result
                list_var.Add(output_chars[0]);
                //list_var[1] = kieu du lieu result
                list_var.Add(GetTypeData(output_chars[1]));
                for (int i = 0; i < input_chars.Length; i += 2)
                {
                    list_var.Add(input_chars[i]);
                    list_var.Add(GetTypeData(input_chars[i + 1]));
                }
            }
            catch
            {
                except_flag = "Invalid Parameter (GetListValue invalid)";
                Console.WriteLine("Loi tai ham GetListValue khi lay gia tri bien");
            }
            
        }
        //generate code hàm nhập
        public void InputVarHandle(ref List<string> list, string name, string input)
        {
            string[] input_chars = input.Split(new[] { ":", "," }, StringSplitOptions.None);
            if (input_chars.Length % 2 != 0 || input_chars.Length < 2)
            {
                except_flag = "Invalid Pre";
                Console.WriteLine("so luong bien input khong dung");
            }
            else
            {
                try
                {
                    //ten ham
                    string name_function = string.Format("\t\tpublic| void |Nhap_{0}|(", name);
                    for (int i = 0; i < input_chars.Length; i += 2)
                    {
                        if (i != 0)
                            name_function += ", ";
                            string type = GetTypeData(input_chars[i + 1]);
                        string var = string.Format("|ref| {0} {1}", type, input_chars[i]);
                        name_function += var;
                    }
                    name_function += ")";
                    list.Add(name_function);
                    list.Add("\t\t{");
                    //noi dung ham
                    for (int i = 0; i < input_chars.Length ; i += 2)
                    {
                        string type = GetTypeData(input_chars[i + 1]);
                        string function = string.Format("\t\t\tConsole|.|WriteLine|(|\"Nhap {0}: \"|);", input_chars[i]);
                        list.Add(function);
                        function = string.Format("\t\t\t{0} = {1}.|Parse|(|Console|.|ReadLine|());", input_chars[i], type);
                        list.Add(function);
                    }
                    list.Add("\t\t}");
                }
                catch
                {
                    except_flag = "Invalid Pre";
                    Console.WriteLine("Lỗi InputVarHandle");
                }
            }    
        }
        //generate code hàm xuất
        public void OutputVarHandle(ref List<string> list, string name, string input)
        {
            string[] output_chars = input.Split(new[] { ":", "," }, StringSplitOptions.None);
            if (output_chars.Length != 2)
            {
                except_flag = "Invalid Post";
                Console.WriteLine("so luong bien output khong dung");
            }
            else
            {
                try
                {
                    //ten ham
                    string type = GetTypeData(output_chars[1]);
                    string name_function = string.Format("\t\tpublic| void| Xuat_{0}|({1} {2})", name, type, output_chars[0]);
                    list.Add(name_function);
                    list.Add("\t\t{");
                    //noi dung ham
                    string function = "\t\t\tConsole|.WriteLine|(|\"Ket qua la : {0}\"|,"; 
                    string tail_function = string.Format(" {0});", output_chars[0]);
                    list.Add(function+ tail_function);
                    list.Add("\t\t}");
                }
                catch
                {
                    except_flag = "Invalid Post";
                    Console.WriteLine("Lỗi OutputVarHandle");
                }
            }
        }
        //lấy kiểu dữ liểu biến đầu vào và biến đầu ra
        public string GetTypeData(string type)
        {
            if (type.Length != 1)
            {
                if (type == "char*")
                    return "|string|";
                else
                    Console.WriteLine("Loi kieu du lieu khong ton tai");
                    return "0";
            }
            else
            {
                switch ((int)type[0])
                {
                    case 66:
                        return "|bool|";
                    case 82:
                        return "|float|";
                    case 90:
                        return "|int|";
                    default:
                        Console.WriteLine("Loi kieu du lieu khong ton tai");
                        return "0";
                }
            }
            
        }
        //xu ly pre để thêm hoặc bớt ngoặc, tránh exception
        public string ExceptionPreHandle(string pre)
        {
            pre = pre.Replace(" ", String.Empty);
            pre = pre.Replace("(", String.Empty);
            pre = pre.Replace(")", String.Empty);
            string[] temp_pre = pre.Split(new[] { "&&" }, StringSplitOptions.None);
            if (temp_pre.Length > 1)
            {
                pre = "(";
                for (int i = 0; i < temp_pre.Length; i++)
                {
                    if (i == 0)
                        pre += string.Format("({0})", temp_pre[0]);
                    else
                        pre += string.Format(" && ({0})", temp_pre[0]);
                }
                pre += ")";
            }
            else
            {
                pre = string.Format("({0})", temp_pre[0]);
            }    
            return pre;
        }
        //xử lý post để thêm hoặc bớt ngoặc, tránh exception
        public string ExceptionPostHandle(string post)
        {
            post = post.Replace(" ", String.Empty);
            post = post.Replace("(", String.Empty);
            post = post.Replace(")", String.Empty);
            string[] expressions = post.Split(new[] { "||" }, StringSplitOptions.None);
            //nếu chỉ có 1 biểu thức
            if (expressions.Length == 1)
            {
                post = string.Format("({0})", expressions[0]);
                return post;
            }
            //nếu có nhiều biểu thức
            else
            {
                //chạy từng biểu thức nhỏ
                post = "(";
                for (int i = 0; i < expressions.Length; i++)
                {
                    Console.WriteLine(post);
                    if (i == 0)
                    {   //mỗi biểu thức nhỏ xử lý giống pre handle exception
                        string[] mini_condition = expressions[i].Split(new[] { "&&" }, StringSplitOptions.None);
                        for (int j=0; j< mini_condition.Length;j++)
                        {
                            if (j==0)
                                post += string.Format("({0})", mini_condition[j]);
                            else
                                post += string.Format("( && {0})", mini_condition[j]);
                        }    
                    }
                    else
                    {
                        post += "||";
                        string[] mini_condition = expressions[i].Split(new[] { "&&" }, StringSplitOptions.None);
                        for (int j = 0; j < mini_condition.Length; j++)
                        {
                            if (j == 0)
                                post += string.Format("({0})", mini_condition[j]);
                            else
                                post += string.Format("( && {0})", mini_condition[j]);
                        }
                    }
                }
                post += ")";
                return post;
            }
        }
        //xử lý pre để đổi màu và in ra lại màn hình textbox output
        public string PreHandleForInput(string pre)
        {
            pre = pre.Replace(" ", String.Empty);
            pre = pre.Replace("(", String.Empty);
            pre = pre.Replace(")", String.Empty);
            if (pre == "")
                return pre;
            string[] temp_pre = pre.Split(new[] { "&&" }, StringSplitOptions.None);
            if (temp_pre.Length > 1)
            {
                pre = "(";
                for (int i = 0; i < temp_pre.Length; i++)
                {
                    if (i == 0)
                        pre += string.Format("({0})", temp_pre[i]);
                    else
                        pre += string.Format(" ~&&~ ({0})", temp_pre[i]);
                }
                pre += ")";
            }
            else
            {
                pre = string.Format("({0})", temp_pre[0]);
            }
            return pre;

        }
        //xử lý post để đổi màu và in ra lại màn hình textbox output
        public string PostHandleForInput(string post)
        {
            post = post.Replace(" ", String.Empty);
            post = post.Replace("(", String.Empty);
            post = post.Replace(")", String.Empty);
            string[] expressions = post.Split(new[] { "||" }, StringSplitOptions.None);
            //nếu chỉ có 1 biểu thức
            if (expressions.Length == 1)
            {
                post = string.Format("({0})", expressions[0]);
            }
            //nếu có nhiều biểu thức
            else
            {
                //chạy từng biểu thức nhỏ
                post = "(";
                for (int i = 0; i < expressions.Length; i++)
                {

                    if (i == 0)
                    {   //mỗi biểu thức nhỏ xử lý giống pre handle exception
                        post += PreHandleForInput(expressions[i]);
                    }
                    else
                    {
                        post += "\r\n       ~||~ " + PreHandleForInput(expressions[i]);
                    }
                }
                post += ")";     
            }
            return post;
        }
    }
}
