using System.Collections;
using UnityEngine;

namespace LUX {
    class TaskManager : MonoBehaviour {
        static TaskManager singleton;
        public static TaskState CreateTask(IEnumerator coroutine) {
            if (singleton == null) {
                GameObject go = new GameObject("TaskManager");
                singleton = go.AddComponent<TaskManager>();
            }
            return new TaskState(coroutine);
        }
        public class TaskState {
            public bool Running {
                get {
                    return running;
                }
            }
            public bool Paused {
                get {
                    return paused;
                }
            }            
            public delegate void FinishedHandler(bool manual);
            public event FinishedHandler Finished;

            IEnumerator coroutine;
            bool running;
            bool paused;
            bool stopped;

            public TaskState(IEnumerator c) {
                coroutine = c;
            }
            public void Pause() {
                paused = true;
            }
            public void Unpause() {
                paused = false;
            }
            public void Start() {
                running = true;
                singleton.StartCoroutine(CallWrapper());
            }
            public void Stop() {
                stopped = true;
                running = false;
            }
            IEnumerator CallWrapper() {
                yield return null;
                IEnumerator e = coroutine;
                while (running) {
                    if (paused)
                        yield return null;
                    else {
                        if (e != null && e.MoveNext()) {
                            yield return e.Current;
                        } else {
                            running = false;
                        }
                    }
                }

                FinishedHandler handler = Finished;
                if (handler != null)
                    handler(stopped);
            }
        }        
    }
}