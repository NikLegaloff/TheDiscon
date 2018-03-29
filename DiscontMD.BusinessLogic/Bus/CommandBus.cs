using System;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DiscontMD.BusinessLogic.DomainModel;
using DiscontMD.BusinessLogic.Presistense.MSSQL;

namespace DiscontMD.BusinessLogic.Bus
{
    public class CommandExecutor
    {
        private readonly Action<string> _logger;

        public CommandExecutor(Action<string> logger)
        {
            _logger = logger;
        }

        public async Task<bool> TakeOneAndExecute()
        {
            var commandName = "unknown";
            var takeId = Guid.NewGuid();
            await MSSqlDb.Execute("update CommandDTO set TakeId='" + takeId +
                                  "', StartedDate=GetDate() where id in (select top 1 Id from CommandDTO where takeid is null order by createddate)");
            var list = await Registry.Current.Data.Commands.Select(" where takeid='" + takeId + "'");
            await MSSqlDb.Execute("update CommandDTO set state=1 where takeid='" + takeId + "'");
            if (list == null || list.Length == 0) return false;
            var commandDTO = list[0];
            try
            {
                var o = JsonConvert.DeserializeObject(commandDTO.BodyJSON,
                    new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All});
                commandName = o.ToString();
                var command = (ICommand) o;
                _logger(commandName + "\tstarted");
                var res = await Execute(command, commandDTO.Id);
                await MSSqlDb.Execute("update CommandDTO set state=" + (res ? 2 : 3) +
                                      ", EndedDate=GetDate() where id='" + commandDTO.Id + "'");
                _logger(commandName + "\t" + (res ? "success" : "faill"));
            }
            catch (Exception e)
            {
                _logger(commandName + "\t" + "exception\t" + e.Message);
                await MSSqlDb.Execute(
                    "update CommandDTO set state=3 and status=@status, EndedDate=GetDate() where id='" + commandDTO.Id +
                    "'", new {status = e.ToString()});
                return true;
            }
            return true;
        }

        private async Task<bool> Execute(ICommand command, Guid id)
        {
            var commandType = command.GetType();
            var name = commandType.FullName + "Handler";
            var assembly = Assembly.GetAssembly(GetType());
            var type = assembly.GetType(name, false, true);
            var inst = Activator.CreateInstance(type, args: id);

            var method = inst.GetType().GetMethod("Process");
            return await (Task<bool>) method.Invoke(inst, new[] {command});
        }
    }

    public class CommandBus
    {
        public static string BusSecret = "12349df238u822ed2903ur";

        public async Task<Guid> Invoke(ICommand command)
        {
            var dto = new CommandDTO();
            dto.BodyJSON = JsonConvert.SerializeObject(command, Formatting.None, new JsonSerializerSettings
            {
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
                TypeNameHandling = TypeNameHandling.All
            });
            dto.State = CommandSatet.Ready;
            dto.CreatedDate = DateTime.Now;
            await Registry.Current.Data.Commands.Save(dto);
            return dto.Id;
        }
    }
}