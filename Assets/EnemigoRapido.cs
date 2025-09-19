using System.Collections;
using System.ComponentModel.Design;
using UnityEngine.AI;
using UnityEngine;

public class EnemigoRapido : moviminetoEnemigo
 {   // Start is called once before the first execution of Update after the MonoBehaviour is created

   [SerializeField] float tiempoDelay = 2f;
   [SerializeField] float velDash = 80f;
   [SerializeField] float tiempoDash = 1f;
   protected override void Atacar()
   {
      colorEnemigo.material = rojoAtaque;
      agent.SetDestination(player.position);
      transform.LookAt(player);
      agent.speed = agent.speed / 12f;
       StartCoroutine(Delay(tiempoDelay));
      agent.enabled = false;
      
    
    
    
    
    
     /* agent.autoBraking = false;
             agent.SetDestination(player.position);
             transform.LookAt(player);
             agent.speed = agent.speed / 12f;
            StartCoroutine(Delay(tiempoDelay));

             agent.speed = velDash;
             agent.acceleration = velDash;
             agent.angularSpeed = agent.angularSpeed / 20f;

            StartCoroutine(Delay(tiempoDash));
             agent.speed = 8f;
             agent.angularSpeed = 120f;
             agent.acceleration = 8f;
             agent.autoBraking = true;
       */
      if (!yaAtaco)
      {

         yaAtaco = true;
         Invoke(nameof(AttackReset), frecuenciaAtaque);

      }

      IEnumerator Delay(float delay)
      {
         yield return new WaitForSeconds(delay);
      }
      
   }
 }