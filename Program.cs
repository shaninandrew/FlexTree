using System.Collections;
using System.Collections.Generic;
using System.Buffers.Text;
using System.Diagnostics;



// See https://aka.ms/new-console-template for more information
Console.WriteLine("FlexTree!");




FlexTree<string> root = new FlexTree<string>("Корешок", 0);
root.AddChild("Кузя", "Петя");
root.left.AddChild("Кузьмич", "Кузьма");
root.rigth.AddChild("Петрович", "Петро");

root.rigth.rigth.AddChild("Петраков", "Пертушка");
root.left.left.AddChild("Кузьмино", "Кузьмичево");


foreach (var s in root.GetAll())
{
    Console.WriteLine($"  - -   {s.Key:D4} : {s.Value} ");
}

string t4 = "";
 if (root.GetValue(4, out t4))
        Console.WriteLine( $" id (4)= {t4}");

string t5 = "";
if (root.GetValue(5, out t5))
    Console.WriteLine($" id (5)= {t5} ");

string t12 = "";
if (root.GetValue(-12, out t12))
    Console.WriteLine($" id (-12)= {t12} ");

t12 = "";
if (root.GetValue(12, out t12))
    Console.WriteLine($" id (12)= {t12} ");

string val=  root.element(5).value;
Console.WriteLine($" element id (5)= {val} ");

Console.WriteLine($"Построение пути от {root.element(-1).value} до {root.element(-12).value}: ");

List<long> list = root.element(-1).Trace(root.element(-12));
foreach (long i in list)
{ Console.Write($" {i} {root.element(i).value} -> " ); }

Console.WriteLine();

root.PrintTree(root);


//------------------------ Код под капотом ------------------------

/// <summary>
/// Дерево из произвольных элементов.
/// Можно
/// </summary>
/// <typeparam name="T">Укажите свой тип для деревьев: string, int, enum, list и т.п.</typeparam>

class FlexTree<T>:IDisposable
{
    public T value { set; get; }
    public long ID = 0;
    public long level = 0;

    private FlexTree<T> parent;
    public FlexTree<T> left;
    public FlexTree<T> rigth;

    /// <summary>
    /// Конструктор дерева
    /// </summary>
    /// <param name="value_">Основное значение</param>
    /// <param name="left_">Левая ветвь</param>
    /// <param name="right_">Правая ветвь</param>
    /// <param name="ID_">Внутренний номер</param>
    public FlexTree(T value_, FlexTree<T> left_, FlexTree<T> right_ ,long ID_ =0)
    {
            value = value_;
            ID = ID_;
            
            this.left = left_;
            this.left.ID = Math.Sign(Math.Abs(ID_)) * Math.Abs(ID_) ;

            this.left.level = level + 1;

            this.rigth = right_;
            this.rigth.ID = Math.Sign(Math.Abs(ID)) * Math.Abs(ID) + Math.Sign(Math.Abs(ID)); 
        
            this.rigth.level = level + 1;

    }

    /// <summary>
    /// Создание узла со значением
    /// </summary>
    /// <param name="value_"></param>
    /// <param name="left_"></param>
    /// <param name="right_"></param>
    public FlexTree(T value_,  long ID_ = 0)
    {
        value = value_;
        this.left = null;
        this.rigth = null;
        this.parent = null;
        ID = 0;
        level = 0;
    }


    /// <summary>
    /// Создает сразу экземпляр со значениями
    /// </summary>
    /// <param name="value_"></param>
    /// <param name="left_"></param>
    /// <param name="right_"></param>
    /// <param name="ID"></param>
    public FlexTree(T value_, T left_, T right_, long ID = 0)
    {
        this.value = value_;
        this.ID = ID;
        this.left =  new FlexTree<T> (left_);
        this.left.parent= this;

        // -1 -2 -3 -> -4 -5 -> -
        this.left.ID = Math.Sign(Math.Abs(ID)) * Math.Abs(ID) + 2*Math.Sign(Math.Abs(ID));
        

        this.rigth = new FlexTree<T>(right_);
        this.rigth.parent = this;
        this.rigth.ID = Math.Sign(Math.Abs(ID)) * Math.Abs(ID) + Math.Sign(Math.Abs(ID)); 
        this.left.level = level + 1;
        this.rigth.level = level + 1;
    }

    /// <summary>
    /// Добавление детской ветви
    /// </summary>
    /// <param name="left_">Левая</param>
    /// <param name="right_">Правая</param>
    public void AddChild(T left_, T right_)
    {
        
        this.left = new FlexTree<T>(left_);
        this.left.parent = this;
        this.left.ID = -Math.Abs( Math.Sign(Math.Abs(ID)) * Math.Abs(ID) *2 + (1+Math.Sign(Math.Abs(ID))));

        this.rigth = new FlexTree<T>(right_);
        this.rigth.parent = this;
        this.rigth.ID = Math.Abs( Math.Sign(Math.Abs(ID)) * Math.Abs(ID) + (2+Math.Sign(Math.Abs(ID))));
        
        this.left.level = level + 1;
        this.rigth.level = level + 1;
    }
    /// <summary>
    /// Удаление объекта из дерева.
    /// Удаляется все целиком.
    /// </summary>
    public void RemoveChild() 
    {
        ID = 0;
        if (left != null)  { left.RemoveChild(); left.Dispose(); }
        if (rigth != null) { rigth.RemoveChild(); rigth.Dispose(); }
    }

    /// <summary>
    /// Ищет конкретное значение - как есть, через Equals
    /// Возвращает индекс в дереве.
    /// </summary>
    /// <param name="find_value">Что ищем?</param>
    /// <returns></returns>
    public long? Find (T find_value)
    {
        long?   FINDED = null;
        if (this.value.Equals( find_value))
        {
            FINDED = ID; 
        }
        else
        {
            long? R = left.Find(find_value);
            FINDED = R;
            if (FINDED!= null ) return (FINDED);

            R = rigth.Find(find_value);
            FINDED = R;
            if (FINDED != null) return (FINDED);
        }
        return (FINDED);
    }

    /// <summary>
    /// Удаление объекта из дерева, с ветками
    /// </summary>
    public void Dispose()
    {
        left.Dispose();
        rigth.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Возвращает значение по индексу в выходной параметр. 
    /// На выходе True/False - нашел или нет
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public bool GetValue(long ID, out T x)
    {
        
        /// оптимизация
        if (this.ID == ID)
        {
            x = this.value;
            return ( true);
        }

        /// Хак: ускорение
        /// Если модуль ID> abs(this.ID) то смысла нет искать глубже
        if (Math.Abs(ID)>= Math.Abs(this.ID))
        {
                
                //ищем по 2 ветвям последовательно
                if (this.rigth != null)
                {
                    bool t = this.rigth.GetValue(ID, out x);
                    if (t) return (t);
                }

                if (this.left != null)
                {
                    bool t = this.left.GetValue(ID,out x);
                    if (t)  return (t);
                }
            }

            x =default(T);
            //коли ничего не нашел
            return (false);
    }// GetValue

    /// <summary>
    /// Дает все значения из дерева в словарь - а-ля редиска на минималках
    /// </summary>
    /// <returns>Словарь из дерева в виде N, значение</returns>
    public Dictionary<long,T> GetAll()
    {
        Dictionary<long, T> res = new Dictionary<long,T>();
        
        if (this.value != null)
        {
            res.Add(this.ID, this.value);
        }

        if (this.left != null)
        {
            foreach (var x in this.left.GetAll())
                res.Add(x.Key,x.Value);
        }
        
        if (this.rigth != null)
        {
            foreach (var x in this.rigth.GetAll())
                res.Add(x.Key, x.Value);
        }

        return (res);

    }


    /// <summary>
    /// Доступ к элементу
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public FlexTree<T> element(long ID)
    { 
        FlexTree<T> tree = null;

        if (this.value != null)
        {
            if (this.ID == ID)
            {
                tree = this;
                return tree;
            }
        }

        if (this.left != null)
        {
            if (this.left.ID == ID)
            {
                tree = this.left;
                return tree;
            }
            else
            { 
                tree = this.left.element(ID);
                if (tree != null) { return tree;  }
            
            }
        }

        if (this.rigth != null)
        {
            if (this.rigth.ID == ID)
            {
                tree = this.rigth;
                return tree;
            }
            else
            { 
                tree = this.rigth.element(ID);
                if (tree != null) { return tree; }
            }
        }
        
        return (tree);
    } // element

    /// <summary>
    /// Поиск маршрута между узлами. Возвращает ID между узлами
    /// </summary>
    /// <param name="tree"></param>
    /// <returns>Список индексов между ветвями </returns>
    public List<long> Trace(FlexTree<T> tree)
    { 
        List<long> res = new List<long>();
        FlexTree<T> p = this;
        FlexTree<T> q = tree;
        
        do
        {
            //Вставка результов сканирования дерева идет через середину, тогда формируется правильный путь
            // от стартового узла до конечного узал
            
            if ((p != null) && (q == null))
            {
                res.Insert(res.Count / 2, p.ID); // если данные в одном узле
            }

            if ((p == null) && (q != null))
            {
                res.Insert(res.Count / 2, q.ID); // если данные в другом
            }

            if ((p == null) || (q == null)) { break; } // похоже все обошли


            if (p.ID == q.ID) // нашли общий узел
                {
                    res.Insert(res.Count/2, p.ID);
                    break;
                }
            //пишем маршрут
            if ((p.ID != q.ID) && (p.left.ID != q.ID) && (p.rigth.ID != q.ID))
            {

                res.Insert(res.Count / 2 , p.ID);
                res.Insert(res.Count / 2, q.ID);

                p = p.parent; // идем в вверх по предкам
                q = q.parent; // идем в вверх по другой ветке
            }
        }
        while  (true);
        return (res); //возвращаем маршрут
    }

    public void PrintTree(FlexTree<T> tree)
    {
        
        FlexTree<T> p = tree;

        string format = "";
        for (int i = 0; i < level; i++)
            format += "-";
        format += "> ";

        if (p != null)
            {

                Console.WriteLine("{1} {2} :{0} ", p.value.ToString(), format, p.ID);
                p.PrintTree(p.left);
                p.PrintTree(p.rigth);
            }


    }

} //class


