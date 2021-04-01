using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Command
{
    /// <summary>
    /// Интерфейс операции
    /// </summary>
   public interface ICommand
   {
       /// <summary>
       /// Выполнение операции
       /// </summary>
       void Execute();
   }

    /// <summary>
    /// Абстрактный класс операции
    /// </summary>
    public abstract class ACommand : ICommand
    {
        /// <summary>
        /// Выполнение операции
        /// </summary>
        public void Execute()
        {
            CM.Instance.Registry(this.Clone());
            this.doExecute();
        }
        protected abstract void doExecute();
        protected abstract ACommand Clone();
    }

    /// <summary>
    /// Класс реализации отмены операции  
    /// </summary>
    public class CM
    {
        /// <summary>
        /// Единственный экземпляр класса
        /// </summary>
        static CM instance = null;

        /// <summary>
        /// Список команд
        /// </summary>
        List<ICommand> cmds = new List<ICommand>();

        /// <summary>
        /// Список отменённых команд
        /// </summary>
        List<ICommand> blacklist = new List<ICommand>();
        bool lock_registry = false;
        CM() { }
        public static CM Instance 
        {
            get
            {
                if (instance == null)
                    instance = new CM();
                return instance;
            }
        }

        /// <summary>
        /// Регистрация команды в списке
        /// </summary>
        /// <param name="cmd"> Команда </param>
        public void Registry(ICommand cmd)
        {
            if (lock_registry)
                return;
            cmds.Add(cmd);
            blacklist.Clear();
        }
        
        /// <summary>
        /// Отмена последней операции
        /// </summary>
        public void Undo()
        {   
            lock_registry = true;
            if(cmds.Count > 1)
            {
                blacklist.Add(cmds.Last());
                cmds.RemoveAt(cmds.Count - 1);
                foreach (ICommand c in cmds)
                    c.Execute();
            }
            lock_registry = false;
        }

        /// <summary>
        /// Возврат последней операции из отменённых
        /// </summary>
        /// <returns> Результат возврата операции </returns>
        public bool Redo()
        {
            if (blacklist.Count > 0)
            {
                lock_registry = true;
                cmds.Add(blacklist.Last());
                blacklist.RemoveAt(blacklist.Count - 1);
                foreach (ICommand c in cmds)
                    c.Execute();
                lock_registry = false;
                return true;
            }
            else return false;
        }
    }
}
